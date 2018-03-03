from flask import Flask, render_template, Response, jsonify, request, abort, redirect
from flask_sqlalchemy import SQLAlchemy
import os
import sys


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
    trans_amount = db.Column(db.Integer, nullable=False)
    trans_to = db.Column(db.String, nullable=True)
    user_id = db.Column(db.String(64), db.ForeignKey('users.user_id'), nullable=False)
    def __repr__(self):
        return '<Transaction %r   %r   %r>' % self.trans_id, self.trans_type, self.current_balance

class User(db.Model):
    __tablename__='users'
    user_id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(128), nullable=True)
    current_balance=db.Column(db.Integer, nullable=True)
    pin = db.Column(db.Integer, nullable=True)
    transacts = db.relationship('Transaction', backref='user')
    def __repr__(self):
        return self.name

#Models end here

db.drop_all()
db.create_all()
default_user = User(user_id=0, 
                    name="Test User", 
                    current_balance=10000,
                    pin=1754)
default_transaction = Transaction(trans_id="321056", 
                    user_id='testuser13', 
                    trans_amount=0, 
                    trans_type='wdr')

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
        if result.pin==form.pin:
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
            res={"status":"0", 'uid':result.user_id, 'transactions':trans}
        else:
            res={"status":"0", "uid":result.user_id,'username':result.name, 'currentBalance':result.current_balance}
    return jsonify(res)
#need to save transaction history
@app.route('/action', methods=['POST'])
def action(specific):
    arr = specific.split("%")
    uid = arr[0].split("=")[1]
    uid2 = None

    if(len(arr)>2):
        uid2=arr[2].split("="[1])

    act = arr[1].split("=")
    res=None
    if(act[0]=="withdraw"):
        res=withdraw(uid, act[1])
    elif(act[0]=="deposit"):
        res=deposit(uid, act[1])
    elif(act[0]=="transfer"):
        res=transfer(uid, uid2, act[1])
    return jsonify(res)

def withdraw(uid, amt):
    result=db.session.query(User).filter_by(user_id=uid)
    actu = result.first()
    if(len(result)==0):
        return {'status':1, 'error':"no user"}

    if(actu.current_balance<=0 or amt>500):
        actu.current_balance=amti
        return {'status':1, 'error':"too much"}
    else:
        amti = actu.current_balance
        actu.current_balance=amti-amt
        size=len(db.session.query(Transaction).all)
        trans = Transaction(user_id=uid, trans_id=size, trans_type='withdraw', trans_amount=amt)
        db.session.add(trans)

    db.session.commit()
    return {'status':0, 'error':''}

def deposit(uid, amt):
    result=db.session.query(User).filter_by(user_id=uid)
    actu = result.first()
    if(len(result)==0):
        return {'status':1, 'error':"no user"}
    amti = actu.current_balance
    if(amt<0):
        actu.current_balance=amti
        return {'status':1, 'error':"too little"}
    else:
        
        actu.current_balance=amti-amt
        size=len(db.session.query(Transaction).all)
        trans = Transaction(user_id=uid, trans_id=size, trans_type='deposit', trans_amount=amt)
        db.session.add(trans)
    db.session.commit()
    return {'status':0, 'error':''}

#might have to use a different updating format
#haven't tested if it works
def transfer(uid, uid2, amt):
    result=db.session.query(User).filter_by(user_id=uid)
    result2=db.session.query(User).filter_by(user_id=uid2)
    actu = result.first()
    actu2 = result2.first()
    if(len(result)==0 or len(result2)==0):
        return {'status':1, 'error':"no user"}
    amti = actu.current_balance
    if(amt<0):
        actu.current_balance=amti
        return {'status':1, 'error':"too little"}
    else:
        
        actu.current_balance=amti-amt
        amti2 = actu2.current_balance
        actu2.current_balance=amti2+amt
        size=len(db.session.query(Transaction).all)
        trans = Transaction(user_id=uid, trans_id=size, trans_type='transfer', trans_amount=amt, trans_to=uid2)
        db.session.add(trans)
        db.session.commit()
    return {'status':0, 'error':''}

def clear():
    os.system('clear')

@app.shell_context_processor
def make_shell_context():
    return dict(app=app, db=db, User=User, Transaction=Transaction, clear=clear)