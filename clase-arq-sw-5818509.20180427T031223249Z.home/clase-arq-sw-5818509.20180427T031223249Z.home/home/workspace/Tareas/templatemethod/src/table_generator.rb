# Template Method Pattern
# Date: 22-Jan-2018
# Author:
#          A01374527 

#The source code contained in this file is used to define de implementation of <b>TableGenerator</b> Class
#and it´s sub-classes

#Class thats models the generation of a table in a general way
class TableGenerator

  #initialize the class with "header" and "data" values
  def initialize(header, data)
    @header = header
    @data = data
  end
  #Returns the complete table in a String
  def generate
    generate_header_row + (@data.map {|x| generate_row(x)}).join
  end
  #Returns the generation of the complete header
  def generate_header_row
    (@header.map {|x| generate_header_item(x)}).join
  end
  #Returns a single item
  def generate_item(item)
    item
  end
  #Returns the complete generation of the rows
  def generate_row(row)
    (row.map {|x| generate_item(x)}).join
  end
  #Returns a single header item
  def generate_header_item(item)
    item
  end

end
#Class extended from TableGenerator that generates the table with a CSV format
class CSVTableGenerator < TableGenerator
  #Returns the generation of the table in CSV format
  def generate_row(row)
    "#{(row.map {|x| generate_item(x)}).join(',')}\n"
  end
  #Returns the header of the CSV table
  def generate_header_row
    generate_row(@header)
  end

end
#Clss extended from TableGenerator that generates a table with HTML format
class HTMLTableGenerator < TableGenerator
  #returns the genaration of the table in HTML format
  def generate
    #[ "a", "b", "c" ].join #=> "abc"
    cadena = "<table>\n#{generate_header_row + (@data.map {|x| generate_row(x)}).join}</table>\n"
    #puts("resultado-----\n" + cadena + "\ntermina-------------\n")
    return cadena
  end
  #Returns the generation of HTML header
  def generate_header_row
    "<tr>#{(@header.map {|x| generate_header_item(x)}).join}</tr>\n"
  end
  #returns a header item
  def generate_header_item(item)
    "<th>#{item}</th>"
  end
  #renturns the generation of HTML rows
  def generate_row(row)
    "<tr>#{(row.map {|x| generate_item(x)}).join}</tr>\n"
  end
  #generate a single item row
  def generate_item(item)
    "<td>#{item}</td>"
  end
  
  
end
#Class extended from TableGenerator that creates a table in Ascii format
class AsciiDocTableGenerator < TableGenerator
  #Returns the complete generation of the table in Ascii format
  def generate
    cadena = "#{generate_header_row + (@data.map {|x| generate_row(x)}).join}|==========\n"
    #puts("resultado-----\n" + cadena + "\ntermina-------------\n")
    return cadena
  end
  #Returns the generation of the rows
  def generate_row(row)
    "#{(row.map {|x| generate_item(x)}).join}\n"
  end
  #Returns a single row item
  def generate_item(item)
    "|#{item}"
  end
  #Return the generation of the header
  def generate_header_row
    "[options=\"header\"]\n|==========\n#{(@header.map {|x| generate_header_item(x)}).join}\n"
  end
  #Returns a single header item
  def generate_header_item(item)
    "|#{item}"
  end
end 