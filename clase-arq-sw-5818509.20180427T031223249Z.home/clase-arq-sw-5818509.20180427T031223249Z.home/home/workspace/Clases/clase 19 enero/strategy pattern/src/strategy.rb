class Student

  attr_reader :id, :name, :gender, :gpa

  def initialize(id, name, gender, gpa)
    @id = id
    @name = name
    @gender = gender
    @gpa = gpa
  end

end

class StudentStrategy

  def execute(array)
    raise 'Abstract method called!'
  end

end

#*CountGenderStrategy*: Strategy for counting the number of students with a certain gender (male or female) in a course.

class CountGenderStrategy < StudentStrategy
  attr_reader :gender, :cuantity
  
  def initialize(gender)
    @gender = gender
    @cuantity = 0
  end
  
  def execute(array)
    @cuantity  = 0
    for x in array
      if(x.gender == @gender)
        @cuantity += 1
      end
    end
    return @cuantity
    #students.select{|s| s.gender == @gender}.size
    
  end

end 

#*ComputeAverageGPAStrategy*: Strategy for computing the average of all the students’ GPA (_Grade Point Average_) scores in a course. 
#Returns `nil` if the course has no students.

class ComputeAverageGPAStrategy < StudentStrategy

  def execute(array)
    if(array.length <= 0)
      return nil
    end
    var = 0
    for x in array
      var+= x.gpa 
    end 
    return var/array.length
  end 
  #return nil if students.empty?
  #students.sum{|s| s.gpa}/ students.size

end
#BestGPAStrategy: Strategy for getting the name of the student with the highest GPA 
#score in a course. Returns nil if the course has no students.

class BestGPAStrategy < StudentStrategy
  def execute(array)
    return nil if (array.empty?)
    array.max_by{|s| s.gpa}.name
    #students.max_by{&:gpa}.name
  end
end


class Course

  def strategy=(new_strategy)
    if !new_strategy.is_a? StudentStrategy
      raise 'Invalid argument. Was expecting a StudentStrategy.'
    end
    @strategy = new_strategy
  end

  def initialize
    @students = []
    @strategy = nil
  end

  def add_student(student)
    @students.push(student)
  end

  def execute
    @strategy.execute(@students)
  end

end

