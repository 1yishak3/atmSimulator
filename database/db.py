from flask import Flask, render_template, Response, jsonify, request, abort, redirect
from flask_sqlalchemy import SQLAlchemy
import os
import sys

#TODO
#handle how transactions are returned
#create the log out on the model
#create the Error and Succeess Handler
basedir = os.path.abspath(os.path.dirname(__file__))

app = Flask(__name__)

app.config.update(
    DEBUG=True,
    SQLALCHEMY_DATABASE_URI='sqlite:///'+os.path.join(basedir, 'data.sqlite'),
    SQLALCHEMY_TRACK_MODIFICATIONS=False,
    SQLALCHEMY_COMMIT_ON_TEARDOWN=True
)

db = SQLAlchemy(app)
# transactions = db.Table('transactions', db.Column('user_id', db.String, db.ForeignKey('users.user_id'), primary_key=True),
                        # db.Column('trans_id', db.String, db.ForeignKey('transactions.trans_id'), primary_key=True))
#Models here

class Transaction(db.Model):
    __tablename__='transactions'
    trans_id = db.Column(
        db.String(64),
        primary_key=True,
        nullable=False)
    trans_type = db.Column(db.String, nullable=False)
    trans_amount = db.Column(db.Float, nullable=False)
    trans_to = db.Column(db.Integer, db.ForeignKey('users.user_id'), nullable=True)
    user_id = db.Column(db.Integer, db.ForeignKey('users.user_id'), nullable=False)
    def __repr__(self):
        return '<Transaction   %s>' % self.trans_id

class User(db.Model):
    __tablename__='users'
    user_id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(128), nullable=True)
    current_balance=db.Column(db.Float, nullable=True)
    pin = db.Column(db.Integer, nullable=True)
    transacts = db.relationship('Transaction', foreign_keys="[Transaction.user_id]", backref='user')
    def __repr__(self):
        return '<User   %s>' % self.name

#Models end here

db.drop_all()
db.create_all()
default_user = User(user_id=0,
                    name="Gamaric Test",
                    current_balance=5000.0,
                    pin=1754)
default_user2 = User(user_id=1,
                    name="Gamaric Test 2",
                    current_balance=5000.0,
                    pin=1754)
default_transaction = Transaction(trans_id="321056",
                    user_id=0,
                    trans_amount=0,
                    trans_type='withdraw')

db.session.add(default_transaction)
db.session.add(default_user)

db.session.commit()

@app.route('/')
def home():
    return render_template('home.html')


@app.route('/login', methods=['GET','POST'])
def login():

    form = request.form

    #response will contain uid to be used in future transactions
    #deal with username uniqueness issue and using that to access database
    result=db.session.query(User).filter_by(name=form["username"]).first()
    res=None
    if result is not None:
        if result.pin==int(form["pin"]):
            res={'status':"0", 'uid':result.user_id,'username':result.name, 'currentBalance':result.current_balance}
        else:
            res={'status':"1",'error':"wrong pin"}
    else:
        res={'status':"1",'error':"no such user"}

    return jsonify(res)


@app.route('/data/<string:uid>/<string:want>', methods=['GET'])
def data(uid, want):
    print(uid)
    print(want)
    res=None
    result= db.session.query(User).filter_by(user_id=uid).scalar()
    if result is None:
        res={'status':1, 'error':"no such user"}
    else:
        if want=="transactions":
            t = result.transacts
            trans={}
            tu=0
            for tr in t:
                obj = {'transId':tr.trans_id,
                'transType':tr.trans_type,
                'userId':tr.user_id,
                'transAmount':tr.trans_amount,
                'transTo':tr.trans_to}
                trans[""+str(tu)]=obj
                tu=tu+1
            res=trans
        else:
            res={"status":"0", "uid":result.user_id,'username':result.name, 'currentBalance':result.current_balance}
    res=jsonify(res)
    print(res)
    return res
#need to save transaction history
@app.route('/action', methods=['POST'])
def action():
    res=None
    result = request.form
    action = result["action"]
    if action=="withdraw":
        res=withdraw(result["userId"], float(result["amount"]))
    elif action=="deposit":
        res=deposit(result["userId"], float(result["amount"]))
    elif action=="transfer":
        res=transfer(result["userId1"], result["userId2"], float(result["amount"]))
    else:
        res={"status":"1", "error":"no such operation"}
    return jsonify(res)

def withdraw(uid, amtl):
    amt=float(amtl)
    result=db.session.query(User).filter_by(user_id=uid).scalar()
    actu = result
    if result is None:
        return {'status':1, 'error':"no user"}
    amti = actu.current_balance
    if(actu.current_balance<=0 or amt>500 or (amti-amt)<0):
        actu.current_balance=amti
        return {'status':1, 'error':"too much"}
    if amt<0:
        actu.current_balance=amti
        return {'status':1, 'error':"too little"}
    elif amt%20!= 0:
        actu.current_balance=amti
        return {'status':1, 'error':"not divisible by 20"}
    else:
        amti = actu.current_balance
        actu.current_balance=amti-amt
        size=len(db.session.query(Transaction).all())
        trans = Transaction(user_id=uid, trans_id=size, trans_type='withdraw', trans_amount=amt)
        db.session.add(trans)

    db.session.commit()
    return {'status':0, 'error':''}

def deposit(uid, amtl):
    amt=float(amtl)
    result=db.session.query(User).filter_by(user_id=uid).scalar()
    actu = result
    if result is None:
        return {'status':1, 'error':"no user"}
    amti = actu.current_balance
    if(amt<0):
        actu.current_balance=amti
        return {'status':1, 'error':"too little"}
    else:
        actu.current_balance=amti+amt
        size=len(db.session.query(Transaction).all())
        trans = Transaction(user_id=uid, trans_id=size, trans_type='deposit', trans_amount=amt)
        db.session.add(trans)
    db.session.commit()
    return {'status':0, 'error':''}

#might have to use a different updating format
#haven't tested if it works
def transfer(uid, uid2, amtl):
    amt=float(amtl)
    result=db.session.query(User).filter_by(user_id=uid).scalar()
    result2=db.session.query(User).filter_by(user_id=uid2).scalar()
    actu = result
    actu2 = result2
    if(result is None or result2 is None):
        return {'status':1, 'error':"no such user"}
    amti = actu.current_balance
    if(amt<0 ):
        actu.current_balance=amti
        return {'status':1, 'error':"too little"}
    if((amti-amt)<0):
        actu.current_balance=amti
        return {'status':1, 'error':"not enough balance"}
    else:
        actu.current_balance=amti-amt
        amti2 = actu2.current_balance
        actu2.current_balance=amti2+amt
        size=len(db.session.query(Transaction).scalar())
        trans = Transaction(user_id=uid, trans_id=size, trans_type='transfer', trans_amount=amt, trans_to=uid2)
        db.session.add(trans)
    db.session.commit()
    return {'status':0, 'error':''}

def clear():
    os.system('clear')

@app.shell_context_processor
def make_shell_context():
    return dict(app=app, db=db, User=User, Transaction=Transaction, clear=clear)
