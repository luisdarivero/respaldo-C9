# Decorator Pattern
# Date: 08-Mar-2018
# Author:
#          A1374527 Luis Daniel Rivero Sosa

# The source code contained in this file demonstrates how to
# implement the <em>Decorator pattern</em>.

require 'minitest/autorun'
require 'coffee'

# A class that test the Beverage class
#and it´s subclasses
class CoffeeTest < Minitest::Test
#method that test Espresso class
  def test_espresso
    beverage = Espresso.new
    assert_equal("Espresso", beverage.description)
    assert_equal(1.99, beverage.cost)
  end
#method that test DarkRoast Class
  def test_dark_roast
    beverage = DarkRoast.new
    beverage = Milk.new(beverage)
    beverage = Mocha.new(beverage)
    beverage = Mocha.new(beverage)
    beverage = Whip.new(beverage)
    assert_equal("Dark Roast Coffee, Milk, Mocha, Mocha, Whip", 
                 beverage.description)
    assert_equal(1.59, beverage.cost)
  end
#Method that test HouseBlend class
  def test_house_blend
    beverage = HouseBlend.new
    beverage = Soy.new(beverage)
    beverage = Mocha.new(beverage)
    beverage = Whip.new(beverage)
    assert_equal("House Blend Coffee, Soy, Mocha, Whip", 
                 beverage.description)
    assert_equal(1.34, beverage.cost)
  end

end