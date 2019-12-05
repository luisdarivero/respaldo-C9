# Domain-Specific Language Pattern
# Date: 15-Mar-2018
# Author:
#          A01374527 Luis Daniel Rivero Sosa


# The source code contained in this file demonstrates how to
# implement the <em>DSL</em>.

# A class that models the behaviour of a Scissor in the game
class Scissors
    #Method that set the behaviour of + operator
    def + arg
        if(arg == Paper)
            puts("Scissors cut Paper (winner Scissors)")
            self
        elsif(arg == Lizard)
            puts("Scissors decapitate Lizard (winner Scissors)")
            self
        elsif(arg == Scissors)
            puts("Scissors tie (winner Scissors)")
            self
        else
            arg + self
        end
    end
    #Method that set the behaviour of - operator
    def - arg
        if(arg == Spock)
            puts("Spock smashes Scissors (loser Scissors)")
            self
        elsif(arg == Rock)
            puts("Rock crushes Scissors (loser Scissors)")
            self
        elsif(arg == Scissors)
            puts("Scissors tie (loser Scissors)")
            self
        else
            arg - self
        end
    end
    #Method that returns the class name
    def s
        "Scissors"
    end
end
# A class that models the behaviour of a Paper in the game
class Paper
    #Method that set the behaviour of + operator
    def + arg
        if(arg == Rock)
            puts("Paper covers Rock (winner Paper)")
            self
        elsif(arg == Spock)
            puts("Paper disproves Spock (winner Paper)")
            self
        elsif(arg == Paper)
            puts("Paper tie (winner Paper)")
            self
        else
            arg + self
        end
    end
    #Method that set the behaviour of - operator
    def - arg
        if(arg == Scissors)
            puts("Scissors cut Paper (loser Paper)")
            self
        elsif(arg == Lizard)
            puts("Lizard eats Paper (loser Paper)")
            self
        elsif(arg == Paper)
            puts("Paper tie (loser Paper)")
            self
        else
            arg - self
        end
    end
    #Method that returns the class name
    def s
        "Paper"
    end
end
# A class that models the behaviour of a Rock in the game
class Rock
    #Method that set the behaviour of + operator
    def + arg
        if(arg == Scissors)
            puts("Rock crushes Scissors (winner Rock)")
            self
        elsif(arg == Lizard)
            puts("Rock crushes Lizard (winner Rock)")
            self
        elsif(arg == Rock)
            puts("Rock tie (winner Rock)")
            self
        else
            arg + self
        end
    end
    #Method that set the behaviour of - operator
    def - arg
        if(arg == Paper)
            puts("Paper covers Rock (loser Rock)")
            self
        elsif(arg == Spock)
            puts("Spock vaporizes Rock (loser Rock)")
            self
        elsif(arg == Rock)
            puts("Rock tie (loser Rock)")
            self
        else
            arg - self
        end
    end
    #Method that returns the class name
    def s
        "Rock"
    end
end
# A class that models the behaviour of a Rock in the game
class Spock
    #Method that set the behaviour of + operator
    def + arg
        if(arg == Rock)
            puts("Spock vaporizes Rock (winner Spock)")
            self
        elsif(arg == Scissors)
            puts("Spock smashes Scissors (winner Spock)")
            self
        elsif(arg == Spock)
            puts("Spock tie (winner Spock)")
            self
        else
            arg + self
        end
    end
    #Method that set the behaviour of - operator
    def - arg
        if(arg == Paper)
            puts("Paper disproves Spock (loser Spock)")
            self
        elsif(arg == Lizard)
            puts("Lizard poisons Spock (loser Spock)")
            self
        elsif(arg == Spock)
            puts("Spock tie (loser Spock)")
            self
        else
            arg - self
        end
    end
    #Method that returns the class name
    def s
        "Spock"
    end
end
# A class that models the behaviour of Lizard in the game
class Lizard
    #Method that set the behaviour of + operator
    def + arg
        if(arg == Paper)
            puts("Lizard eats Paper (winner Lizard)")
            self
        elsif(arg == Spock)
            puts("Lizard poisons Spock (winner Lizard)")
            self
        elsif(arg == Lizard)
            puts("Lizard tie (winner Lizard)")
            self
        else
            arg + self
        end
    end
    #Method that set the behaviour of - operator
    def - arg
        if(arg == Scissors)
            puts("Scissors decapitate Lizard (loser Lizard)")
            self
        elsif(arg == Rock)
            puts("Rock crushes Lizard (loser Lizard)")
            self
        elsif(arg == Lizard)
            puts("Lizard tie (loser Lizard)")
            self
        else
            arg - self
        end
    end
    #Method that returns the class name
    def s
        "Lizard"
    end
end
# Method that evaluates the complete game taing in care a series of arguments
def show(arg)
    print ("Result = #{arg.s}\n")
end
#Initialize of Paper variable
Paper = Paper.new
#Initialize of Scissor variable
Scissors = Scissors.new
#Initialize of Lizard variable
Lizard = Lizard.new 
#Initialize of Spock variable
Spock = Spock.new
#Initialize of Rock variable
Rock = Rock.new

