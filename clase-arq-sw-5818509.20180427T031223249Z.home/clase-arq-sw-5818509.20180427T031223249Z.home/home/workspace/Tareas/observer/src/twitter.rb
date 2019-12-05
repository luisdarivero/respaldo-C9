# Observer Pattern
# Date: 26-Jan-2018
# Authors:
#          A01374527 Luis Daniel Rivero Sosa

#The source code contained in this file demonstrates how the Class Twitter works

#The Class Twitter models a single version of the popular social media "Twitter". 
#This class use the observer pattern to comunicate the tweets to other objects
class Twitter
    #Initialize the instance variables
    def initialize(user)
        @user = user
        @followers = []
        @t = nil
    end
    #Method to allows the object follow another object 
    def follow(u)
        
        u.assignFollower(self)
        return self
    end
    #Save the last tweet and call the observer
    def tweet(t)
        @t = t
        @followers.each do |observer|
            observer.read(self)
        end 
    end
    #Method that asign followers to the object
    def assignFollower(o)
        @followers << o
    end
    #return user variable
    def getUser()
        @user
    end
    #return t variable
    def getT()
        @t
    end
    #Method that print the tweet received and from whom
    def read(o)
        puts "#{@user} received a tweet from #{o.getUser}: #{o.getT}\n"
    end
    
end