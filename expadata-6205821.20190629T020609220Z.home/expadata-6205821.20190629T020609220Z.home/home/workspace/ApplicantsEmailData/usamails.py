import requests
import json
import urllib2
token = "f412e601b8eef7a8536f5c83e22bb29aa147de0d9ec365f205ff8c2fbc558de0"

#funcion que te regresa el email de un aplicante
def applicantMail(oppID,traineeID):
    r = requests.get("https://gis-api.aiesec.org/v2/opportunities/"+str(oppID)+"/applicant?access_token=" + token + "&person_id=" + str(traineeID)) 
    content =  r.content
    data = json.loads(content)
    resultado = ""
    resultado += data["email"] + "," + data["full_name"] + "," + data["home_lc"]["country"]
    #print(str(oppID) + "," + str(traineeID))
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
def getPeople(oppId, numberOfPages, resultado, nombre_archivo):
    #resultado = ""
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
                    
                    if(True):
                        
                        try:
                            
                            dato = applicantMail(oppId,application["person"]["id"]) + "\n"
                            resultado += dato
                            text_file = open("resultadoApsInfo.txt", "w")
                            text_file.write(resultado.encode('utf-8').strip())
                            text_file.close()
                            print (dato)
                            
                        except Exception as e: 
                            print ("error =" + dato + " - " + e)
                            
                except Exception as e:
                    print("Error!!!: " + e)
                    
                    
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
       encabezados = "Email,Name,Country\n"
       resultado += encabezados
       while line:
            oppNumber = line.replace("\n","")
            resultado = getPeople(oppNumber,numberOfPages(oppNumber),resultado,"resultadoApsInfo.txt")
            print line.replace("\n","")
            line = fp.readline()
#llamando a la funcion principal
main()