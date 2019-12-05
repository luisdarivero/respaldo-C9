# coding=utf-8
from tweet import Tweet
import tweepy


class Twitter:
    
    #variables de acceso a tweeter
    consumer_key = ""
    consumer_secret = ""
    key = ""
    secret = ""
    
    #variable que guarda el acceso al API
    api = None
    
    #variables pata manejar los errores
    existError = False
    error = ""
    
    #Constructor que recibe las llaves para acceder a twitter
    def __init__(self, consumer_key,consumer_secret,
                    key,secret):
                        
        #Se guardan las variables de acceso en Tweeter
        self.consumer_key = consumer_key
        self.age = consumer_secret
        self.key = key
        self.secret = secret
        
        #Se pide el acceso a Tweeter
        try:
            auth = tweepy.OAuthHandler(consumer_key,consumer_secret)
            auth.set_access_token(key,secret)
            self.api = tweepy.API(auth)
        except Exception as e: 
            self.error = str(e)
            self.existError = True
            
    #función que obtiene los datos de un tweet a partir del ID
    def getTweetByStatus(self, status):
        try:
            twInfo = self.api.get_status(id=status,tweet_mode="extended")
            tweet =  Tweet(twInfo)
            return tweet
        except Exception as e:
            print (e)
            return None
            
    #Función que regresa una cantidad n de tweets a partir del 
    #Hometimeline de un usuario
    
    def getUserTimeline(self, screenName, tweetsCount):
        try:
            new_tweets = self.api.user_timeline(screen_name = screenName,count=tweetsCount,tweet_mode="extended")
            return self.listTweets(new_tweets)
        except Exception as e:
            print (e)
            return None
    
    #función que dada una lista de tweets regresa una lista de objetos tipo Tweet
    def listTweets(self, tweetList):
        try:
            squared = list(map(lambda x: Tweet(x).copy(), tweetList))
            return squared
            
        except Exception as e:
            print (e)
            return None
            
    def createATweet(self,tweet):
        try:
            newT = Tweet(tweet)
            return newT
        except Exception as e:
            print (e)
            return None
            
    def sortTweetbyAttr(self, alist, attr):
        try:
            exchanges = True
            passnum = len(alist)-1
            while passnum > 0 and exchanges:
               exchanges = False
               for i in range(passnum):
                   if int(alist[i].getAttr(attr))<int(alist[i+1].getAttr(attr)):
                       exchanges = True
                       temp = alist[i]
                       alist[i] = alist[i+1]
                       alist[i+1] = temp
               passnum = passnum-1
            return(alist)
        except Exception as e:
            print (e)
            return None
