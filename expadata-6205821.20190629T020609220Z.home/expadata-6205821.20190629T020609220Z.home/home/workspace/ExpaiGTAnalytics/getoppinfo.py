import requests
import json

token = "8947c4e54832136f9134a4547225121b3ac6f5ac86136f215ed54aceea488c6a"

def oppInfo(idopp):
    r = requests.get("https://gis-api.aiesec.org/v2/opportunities/"+str(idopp)+"?access_token=" + token) 
    content =  r.content
    data = json.loads(content)
    resultado = ""
    resultado += "," + ((data["branch"]["name"]).replace(",","-"))
    resultado += "," + (data["created_at"])[:10]
    resultado += "," + ((data["title"]).replace(",","-"))
    resultado += "," + data["office"]["name"]
    return resultado

print oppInfo(981020)
