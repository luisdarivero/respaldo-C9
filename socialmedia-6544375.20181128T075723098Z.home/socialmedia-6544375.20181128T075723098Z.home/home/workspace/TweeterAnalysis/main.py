# coding=utf-8
from twitter import Twitter

    

twitter = Twitter("5xRp0cwIufdzsen8XaXZMySzA", 
                    "kU25DDuCFG6UKyYZZ5pnh8XLOseJOEzKszP4Id76UtPWFuAHhj",
                    "917251865623924736-dnDnt0A5Ix4HG6ltYx8Y5J9YbbjVldQ",
                    "ICRRasyqfJJ1O8SMRnC5BHis4FhdVXVpEQ1eWlSmMqNyA")

tweets = twitter.getUserTimeline("AIESEC_PA",200)
#print(tweets[0].getAttr("favorite_count"))
newList = twitter.sortTweetbyAttr(tweets, "retweet_count")
print(len(newList))
newList = filter(lambda x: not x.isRT(), newList)
print(len(newList))
#newList.reverse()
#print(newList[0].getAttr("full_text"))
for x in range(0,10): 
    tweet = newList[x]
    print("----------\n" + 
            tweet.getAttr("full_text") + "\n" +
            str(tweet.getAttr("created_at"))+ "\n" +
            str(tweet.getAttr("favorite_count"))+ "\n" +
            str(tweet.getAttr("retweet_count")) +
            "\n-------")
            
