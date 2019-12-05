# Template Method Pattern
# Date: 22-Jan-2018
# Author:
#          A01374527 

#The source code contained in this file is used to define de implementation of the <b>Student</b> Class

#A class that models a student
class Student

  include Enumerable
  # Note: This class does not support the max, min, 
  # or sort methods.

    #initialize the class with values of "id","name" and "grades"
  def initialize(id, name, grades)
    @id = id
    @name = name
    @grades = grades
  end
    #inspect the variables
  def inspect
    "Student(#{@id}, #{@name.inspect})"
  end
    #return the average of the student grades
  def grade_average
    @grades.inject(:+)/@grades.size
  end
    #do a each method 
  def each &block
    yield @id
    yield @name
    @grades.each(&block)
    yield grade_average
  end

end