

import tweepy

auth = tweepy.OAuthHandler("5xRp0cwIufdzsen8XaXZMySzA", "kU25DDuCFG6UKyYZZ5pnh8XLOseJOEzKszP4Id76UtPWFuAHhj")
auth.set_access_token("917251865623924736-dnDnt0A5Ix4HG6ltYx8Y5J9YbbjVldQ", "ICRRasyqfJJ1O8SMRnC5BHis4FhdVXVpEQ1eWlSmMqNyA")

api = tweepy.API(auth)
#a = api.get_status(912886007451676672, tweet_mode='extended')
tweets = api.home_timeline()
#print(tweets[0].user)
print("---------------------------")
#print(dir(tweets[0]))
print("---------------------------")
#print dir(tweets[1])
#print tweets[1].author.description
#print tweets[1].text
#print tweets[1].text
#print(dir(tweets[0]))
#print tweets[0]

#obtiene mi bandeja principal
#1065452567675600897

tweet = api.get_status(id="1064755104967000065",tweet_mode="extended")
print(dir(tweet))
print(tweet.created_at)
print(dir(tweet.author))
#
"""
public_tweets = api.home_timeline(tweet_mode="extended")
print dir(public_tweets[0])
#print public_tweets
for tweet in public_tweets:
    print("---------------------------")
    print tweet.truncated
    print tweet.full_text
    print tweet.author.description
    print tweet.id
    print tweet.id_str
    print("---------------------------")"""

#obtiene un usuario
#user = api.get_user('garciarJAQUI')
#print ((user._json).encode('utf-8').strip())


"""
#obtiene mis amigos
for friend in tweepy.Cursor(api.user_timeline, id="Dian_TL").items():
    # Process the friend here
    var = str(friend)
    print var.encode('utf-8').strip()"""
    
"""
#revisa trends
trends = api.trends_available()

#print str(trends).encode('utf-8').strip()

realt = api.trends_place(23424982)
print realt"""
