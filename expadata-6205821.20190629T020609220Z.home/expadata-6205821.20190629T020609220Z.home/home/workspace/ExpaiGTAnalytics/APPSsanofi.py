
import requests
import json
token = "1d450b14e8a4c6b342d492f9bcb5b2c230b13e6fd73af16d052b3a12aba5def6"

#funcion que te regresa el url a un cv de un aplicante
def cvURL(oppID,traineeID):
    r = requests.get("https://gis-api.aiesec.org/v2/opportunities/"+str(oppID)+"/applicant?access_token=" + token + "&person_id=" + str(traineeID)) 
    content =  r.content
    data = json.loads(content)
    resultado = ""
    resultado += data["cv_url"]
    
    return resultado

#funcion que regresa el numero de paginas necesarias
def numberOfPages(idOpp):
    url = "https://gis-api.aiesec.org/v2/opportunities/"+idOpp+"/applications?access_token=" + token
    r = requests.get(url) 
    content =  r.content
    data = json.loads(content)
    return int(json.dumps(data["paging"]["total_pages"]))
#funcion que guarda la info de los perfiles en un estatus diferente
#a open o withdrawn o rejected
def getPeople(oppId, numberOfPages):
    resultado = ""
    for x in range(1, numberOfPages + 1):
        url = "https://gis-api.aiesec.org/v2/opportunities/"+oppId+"/applications?access_token="+token+"&page="+str(x)
        r = requests.get(url) 
        content =  r.content
        data = json.loads(content)
        #ciclo que analiza cada aplicion por separado
        for application in (data["data"]):
            app = application["current_status"]
            if(app == "open"):
                try:
                    nombreOpp = oppInfo(oppId)
                    nombreEP = application["person"]["full_name"]
                    paisEP = (application["person"]["home_lc"])["country"]
                    listaPaises = ["Argentina","Brazil","Colombia","Costa Rica","Mexico","Peru","Nicaragua","Spain"]
                    if(paisEP in listaPaises):
                        dato = nombreOpp + "," + nombreEP + "," + paisEP
                        print (dato)
                        print(cvURL(oppId,application["person"]["id"]))
                        resultado += dato + "/n"
                except:
                    return "error"
    return resultado
                
#funcion que regresa la info de una opp
def oppInfo(idopp):
    r = requests.get("https://gis-api.aiesec.org/v2/opportunities/"+str(idopp)+"?access_token=" + token) 
    content =  r.content
    data = json.loads(content)
    resultado = ""
    
    resultado += ((data["title"]).replace(",","-"))
    
    return resultado
#definicion de la funcion principal
def main():
    #se lee el archivo
    filepath = 'prueba'  
    with open(filepath) as fp:  
       line = fp.readline()
       resultado = ""
       encabezados = "opID, company, date of oppen, opp tittle, Host committe, appID,startDay,endDay,status,current_status,dayCreated,dayUpdated,epName,DoB,comitee,country\n"
       resultado += encabezados
       while line:
            oppNumber = line.replace("\n","")
            resultado += getPeople(oppNumber,numberOfPages(oppNumber))
            #resultado += getPeople(oppNumber,numberOfPages(oppNumber)).encode('utf-8').strip()
            print line.replace("\n","")
            text_file = open("resultadoApsInfo.txt", "w")
            text_file.write(resultado.encode('utf-8').strip())
            text_file.close()
            line = fp.readline()
#llamando a la funcion principal
main()