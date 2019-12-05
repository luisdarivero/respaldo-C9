# Adapter Pattern
# Date: 22-Feb-2018
# Authors:
#          A01374527 Luis Daniel Rivero Sosa

# The source code contained in this file implements the adapter pattern
# with a SimpleQueue class


#Class that implements the adapter pattern with SimpleQue, adapting to behave as Last-In First-Out (LIFO)
class QueueAdapter
  #Initializes a new stack, using q as the adaptee. 
    def initialize(q)
        @q = q
    end
    #Inserts x at the top of this stack. Returns this stack. 
    def push(x)
        temp = []
        temp << x
        while !@q.empty?
            temp<<@q.remove
        end
        
        
        for element in temp
            @q.insert(element)
        end
        return self
        
    end 
    #Returns nil if this stack is empty, otherwise removes and returns its top element. 
    def pop
        if @q.empty?
          nil
        else
          @q.remove
        end
    end
    #Returns nil if this stack is empty, otherwise returns its top element without removing it. 
    def peek
        if @q.empty?
          nil
        else
          temp = []
          while !@q.empty?
              temp<<@q.remove
          end
          
          
          for element in temp
              @q.insert(element)
          end
          return temp[0]
        end
    end
    #Returns true if this stack is empty, otherwise returns false. 
    def empty?
      @q.empty?
    end
    #Returns the number of elements currently stored in this stack. 
    def size
      @q.size
    end
    
end 