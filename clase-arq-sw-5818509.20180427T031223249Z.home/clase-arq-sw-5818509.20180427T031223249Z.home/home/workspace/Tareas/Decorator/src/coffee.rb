# Decorator Pattern
# Date: 08-Mar-2018
# Author:
#          A1374527 Luis Daniel Rivero Sosa

# The source code contained in this file demonstrates how to
# implement the <em>Decorator pattern</em> with the Beverage example.


#Class that implements a Beverage 
class Beverage
    #No implemented method
    def initialize
        
    end 
    #No implemented method
    def description
        
    end
    #No implemented method
    def cost
        
    end
end 
#class that implements Dark roast coffee
class DarkRoast < Beverage
    
    #Define the Dark Roast Cofee description
    def description
        "Dark Roast Coffee"
    end
    #Define the Dark Roast coffee cost
    def cost
        0.99
    end
end
#class that implements Espresso coffee
class Espresso < Beverage
    #Define the Espresso Cofee description
    def description
        "Espresso"
    end
    #Define the Espresso coffee cost
    def cost
        1.99
    end
end
#class that implements House Blend coffee
class HouseBlend < Beverage
    
    #define the House Blend coffee description
    def description
       "House Blend Coffee" 
    end
    #Define the House Blend coffee cost
    def cost
        0.89
    end
end
#Define the class Condiment Decorator
class CondimentDecorator < Beverage
    #Method that initialize a value with an specific coffee
    def initialize(cofee)
        @cafePred = cofee
    end
    #Method that ask for the coffee description
    def description
       @cafePred.description 
    end
    #method that ask for the cost of coffee
    def cost
        @cafePred.cost
    end
end
#Method that define Mocha
class Mocha < CondimentDecorator
    #Method that initialize the class with a coffee
    def initialize(cofee)
        super(cofee)
    end
    #Method that concatenate full description of coffee
    def description
       super +  ", Mocha"
    end
    #Method that retun the full cost of the coffee
    def cost
        super + 0.20
    end
end
#Method that define Whip 
class Whip < CondimentDecorator
    #Method that initialize the class with a coffee
    def initialize(cofee)
        super(cofee)
    end
    #Method that concatenate full description of coffee
    def description
       super +  ", Whip"
    end
    #Method that retun the full cost of the coffee
    def cost
        super + 0.10
    end
end
#Method that define soy
class Soy < CondimentDecorator
    #Method that initialize the class with a coffee
    def initialize(cofee)
        super(cofee)
    end
    #Method that concatenate full description of coffee
    def description
       super +  ", Soy"
    end
    #Method that retun the full cost of the coffee
    def cost
        super + 0.15
    end
end
#Method that define milk
class Milk < CondimentDecorator
    #Method that initialize the class with a coffee
    def initialize(cofee)
        super(cofee)
    end
    #Method that concatenate full description of coffee
    def description
       super +  ", Milk"
    end
    #Method that retun the full cost of the coffee
    def cost
        super + 0.10
    end
end