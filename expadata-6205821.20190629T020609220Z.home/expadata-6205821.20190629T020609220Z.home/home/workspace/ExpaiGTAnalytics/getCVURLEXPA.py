import requests
import json

token = "1d450b14e8a4c6b342d492f9bcb5b2c230b13e6fd73af16d052b3a12aba5def6"

def cvURL(oppID,traineeID):
    r = requests.get("https://gis-api.aiesec.org/v2/opportunities/"+str(oppID)+"/applicant?access_token=" + token + "&person_id=" + str(traineeID)) 
    content =  r.content
    data = json.loads(content)
    resultado = ""
    resultado += data["cv_url"]
    
    return resultado

print cvURL(1039000,1700134)