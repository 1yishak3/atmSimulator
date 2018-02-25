from flask import Flask, render_template, Response, request, abort, redirect
from flask_sqlalchemy import SQLAlchemy
import os

db = SQLAlchemy()

basedir = os.path.abspath(os.path.dirname(__file__))

app = Flask(__name__)

app.config.update(
    DEBUG=True,
    SQLALCHEMY_DATABASE_URI='sqlite:///'+os.path.join(basedir, 'data.sqlite'),
    SQLALCHEMY_TRACK_MODIFICATIONS=False,
    SQLALCHEMY_COMMIT_ON_TEARDOWN=True
)

db.init_app(app)

#Models here
class User(db.Model):
    __tablename__='users'
    user_id = db.Column(db.String(64), unique=True, primary_key=True, nullable=False)
    name = db.Column(db.String(128), unique=False)
    current_balance=db.Column(db.Integer)
    pin = db.Column(db.Integer)
    

    def __repr__(self):
        return '<User %r   %r   %r>' % self.user_id, self.name, self.current_balance

class Transaction(db.Model):
    __tablename__='transactions'
    user_id = db.Column(db.String(64))
    trans_id = db.Column(
        db.String(64), 
        unique=True, 
        primary_key=True, 
        nullable=False)
    trans_type = db.Column(db.String, nullable=False)
    trans_amount = db.Column(db.Integer, nullable=False)
    trans_to = db.Column(db.String, nullable=True)
    def __repr__(self):
        return '<Transaction %r   %r   %r>'{self.trans_id, self.trans_type, self.current_balance}
#Models end here
db.create_all()
default_user = User(user_id="testuser13", 
                    name="Test User", 
                    current_balance=10000)
default_transaction = Transaction(trans_id=, 
                    user_id='testuser13', 
                    trans_amount=0, 
                    trans_type='wdr')
#create-all() command
@app.route('/')
def home():
    return render_template('home.html')

#this login process might not be implemented properly. 
#we definitely won't be implementing flask_login
#due mainly to time.
@app.route('/login/<path:upd>', methods=['POST'])
def login(upd):
    upwd=upd.split("%")
    username=upwd[0].split("=")[1]
    pin=upwd[1].split("=")[1]
    #response will contain uid to be used in future transactions
    #deal with username uniqueness issue and using that to access database
    result=db.session.query().filter_by(name=username).first()
    res=None
    if(result==username):
        #we will also not be hashing pins
        if(result.pin==pin):
            res={'status':0, 'uid':result.uid, 'name':result.name,'currentBalance':result.current_balance 'error':''}
        else:
            res={'status':1, 'error':"invalid pin"}
    else:
        res={'status':1, 'error':"no user"}

    return Response(res)

#to be filled out according to the data needs of Neke
@app.route('/data/<string:wyw>', methods=['GET'])
def data(wyw):
    inst=wyw.split("%")
    res=None
    if():
    
    elif:

    elif:

    return Response(res)
#need to save transaction history
@app.route('/action/<string:specific>')
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
    return Response(res)

def withdraw(uid, amt):
    result=db.session.query(User).filter_by(user_id=uid)
    actu = result.first()
    if(len(result)=0):
        return {'status':1, 'error':"no user"}

    if(actu.current_balance<0||amt>500):
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
    if(len(result)=0):
        return {'status':1, 'error':"no user"}
    
    if(amt<0):
        actu.current_balance=amti
        return {'status':1, 'error':"too little"}
    else:
        amti = actu.current_balance
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
    if(len(result)==0||len(result2)==0):
        return {'status':1, 'error':"no user"}
   
    if(amt<0):
        actu.current_balance=amti
        return {'status':1, 'error':"too little"}
    else:
        amti = actu.current_balance
        actu.current_balance=amti-amt
        amti2 = actu2.current_balance
        actu2.current_balance=amti2+amt
        size=len(db.session.query(Transaction).all)
        trans = Transaction(user_id=uid, trans_id=size, trans_type='transfer', trans_amount=amt, trans_to=uid2)
        db.session.add(trans)
        db.session.commit()
    return {'status':0, 'error':''}
