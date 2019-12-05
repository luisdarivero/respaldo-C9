import requests
import json
import urllib2
token = "a77437435c284f176d7ec4c5439bda3a1b936c179560970179fef80735e40d89"

#funcion que descarga un cv con un url
def download_file(download_url, name):
    response = urllib2.urlopen(download_url)
    file = open(name + ".pdf", 'w')
    file.write(response.read())
    file.close()
    

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
    #variable don de se regresa el resultado final
    resultado = ""
    #Variables que cuentan los datos 
    aplicantesTotales = 0
    rejected = 0
    favourite = 0
    opens = 0
    withdrawn = 0
    apd = 0
    nombreOpp = oppInfo(oppId)
    for x in range(1, numberOfPages + 1):
        url = "https://gis-api.aiesec.org/v2/opportunities/"+oppId+"/applications?access_token="+token+"&page="+str(x)
        r = requests.get(url) 
        content =  r.content
        data = json.loads(content)
        #ciclo que analiza cada aplicion por separado
        for application in (data["data"]):
            app = application["current_status"]
            aplicantesTotales += 1
            try:
                if(app == "rejected"):
                    rejected += 1
                elif(app == "withdrawn"):
                    withdrawn += 1
                elif(app == "open"):
                    
                    if(application["favourite"] == True):
                        opens += 1
                    else:
                        opens += 1
                else:
                    apd +=1
                        
            except:
                return "error"
    extraInfo = oppExtraInfo(oppId)
    
    resultado = str(oppId) + "," + nombreOpp + "," + extraInfo + "," + str(aplicantesTotales) 
    resultado += "," + str(opens) + "," + str(rejected) 
    resultado += "," + str(withdrawn) + "," + str(apd) +"\n"
    return resultado
                
#funcion que regresa el nombre de una opp
def oppInfo(idopp):
    r = requests.get("https://gis-api.aiesec.org/v2/opportunities/"+str(idopp)+"?access_token=" + token) 
    content =  r.content
    data = json.loads(content)
    resultado = ""
    
    resultado += ((data["title"]).replace(",","-"))
    
    return resultado
    
#funcion que regresa el nombre de la empresa de una opp y la fecha de creacion
def oppExtraInfo(idopp):
    r = requests.get("https://gis-api.aiesec.org/v2/opportunities/"+str(idopp)+"?access_token=" + token) 
    content =  r.content
    data = json.loads(content)
    resultado = ""
    
    resultado += ((data["branch"]["name"]).replace(",","-")) + ","
    resultado += (data["host_lc"]["name"]) + ","
    resultado += (data["created_at"])[:10]
    
    
    return resultado
#definicion de la funcion principal
def main():
    #se lee el archivo
    filepath = 'prueba'  
    with open(filepath) as fp:  
       line = fp.readline()
       resultado = ""
       encabezados = "opID, oppName, Company, Comitee,  DayCreated, TotalApplicants, openApplicants, rejected, withdrawn, approval\n"
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