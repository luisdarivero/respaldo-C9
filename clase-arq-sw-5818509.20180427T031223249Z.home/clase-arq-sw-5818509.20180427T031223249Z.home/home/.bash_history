rm -fR doc
rdoc -m README.rdoc -o doc -f hanna src/*.rb README.rdoc
cp -R img doc/files
ls
cd src/
ls
ruby -I . -w coffee_test.rb 
