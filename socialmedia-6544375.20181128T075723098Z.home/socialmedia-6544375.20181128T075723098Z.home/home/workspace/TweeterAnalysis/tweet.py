# coding=utf-8

import tweepy

class Tweet:
    #diccionario que almacena todos los atributos del tweet
    tweet = {}
    
    #Variables que ayudan a controlar los errores
    existError = False
    error = ""
    
    #constructor, recibe un tweet desde el api
    def __init__(self, tweet):
        try:
            self.tweet["author"] = tweet.author
            self.tweet["contributors"] = tweet.contributors
            self.tweet["coordinates"] = tweet.coordinates
            self.tweet["created_at"] = tweet.created_at
            self.tweet["destroy"] = tweet.destroy
            self.tweet["display_text_range"] = tweet.display_text_range
            self.tweet["entities"] = tweet.entities
            self.tweet["favorite"] = tweet.favorite
            self.tweet["favorite_count"] = tweet.favorite_count
            self.tweet["favorited"] = tweet.favorited
            self.tweet["full_text"] = tweet.full_text
            self.tweet["geo"] = tweet.geo
            self.tweet["id"] = tweet.id
            self.tweet["id_str"] = tweet.id_str
            self.tweet["in_reply_to_screen_name"] = tweet.in_reply_to_screen_name
            self.tweet["in_reply_to_status_id"] = tweet.in_reply_to_status_id
            self.tweet["in_reply_to_status_id_str"] = tweet.in_reply_to_status_id_str
            self.tweet["in_reply_to_user_id"] = tweet.in_reply_to_user_id
            self.tweet["in_reply_to_user_id_str"] = tweet.in_reply_to_user_id_str
            self.tweet["is_quote_status"] = tweet.is_quote_status
            self.tweet["lang"] = tweet.lang
            self.tweet["parse"] = tweet.parse
            self.tweet["parse_list"] = tweet.parse_list
            self.tweet["place"] = tweet.place
            self.tweet["retweet"] = tweet.retweet
            self.tweet["retweet"] = tweet.retweet
            self.tweet["retweet_count"] = tweet.retweet_count
            self.tweet["retweeted"] = tweet.retweeted
            self.tweet["retweets"] = tweet.retweets
            self.tweet["source"] = tweet.source
            self.tweet["source_url"] = tweet.source_url
            self.tweet["truncated"] = tweet.truncated
            self.tweet["user"] = tweet.user
        except Exception as e: 
            self.error = str(e)
            self.existError = True
        
    #metodo para obtener un atributo según una cadena
    def getAttr(self, attr):
        try:
            if attr in self.tweet:
                return self.tweet[attr]
            else:
                return None
        except Exception as e: 
            self.error = str(e)
            self.existError = True
        
    #metodo para preguntar si hay un error
    def isExistError(self):
        return self.existError
    
    #metodo para imprimir el error de creación de la clase
    def getError(self):
        return self.error
        
    #metodo para crear una copia
    def copy(self):
        try:
            nuevo = Tweet(None)
            nuevo.tweet = self.tweet.copy()
            nuevo.error = self.error
            nuevo.existError = self.existError
            return nuevo
        except Exception as e: 
            self.error = str(e)
            self.existError = True
            return None
        
    def isRT(self):
        try:
            tweetText = str(self.getAttr("full_text")[:4].encode("utf-8"))
            if (tweetText.find("RT @") >= 0):
                return True
            return False
        except Exception as e: 
            self.error = str(e)
            self.existError
            return None