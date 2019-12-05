# Adapter Pattern
# Date: 22-Feb-2018
# Authors:
#          A01374527 Luis Daniel Rivero Sosa


# The source code contained in this file describes the funcionality
#of a simple queue


# A class that models a simple queue
class SimpleQueue
#method that initialize an empty list
  def initialize
    @info =[]
  end
#Method that Inserts x at the back of this queue. Returns this queue. 
  def insert(x)
    @info.push(x)
    self
  end
#Method that Removes and returns the element at the front of this queue. 
#Raises an exception if this queue happens to be empty. 
  def remove
    if empty?
      raise "Can't remove if queue is empty"
    else
      @info.shift
    end
  end
#Returns true if this queue is empty, otherwise returns false.
  def empty?
    @info.empty?
  end
#Returns the number of elements currently stored in this queue. 
  def size
    @info.size
  end
#Returns a string representarion of the queue
  def inspect
    @info.inspect
  end

end