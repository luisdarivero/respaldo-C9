from urllib2 import Request, urlopen

request = Request("http://gis.aiesec.org/opportunities?")

response_body = urlopen(request).read()
print response_body