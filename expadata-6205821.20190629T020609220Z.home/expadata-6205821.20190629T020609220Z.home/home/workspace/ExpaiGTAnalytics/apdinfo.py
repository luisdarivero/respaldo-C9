
import requests
import json
token = "8947c4e54832136f9134a4547225121b3ac6f5ac86136f215ed54aceea488c6a"

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
            if(app != "open" and app != "rejected" and app != "withdrawn"):
                resultado += oppId
                resultado += oppInfo(oppId)
                resultado += "," + str(application["id"])
                try:
                    resultado += "," + (application["experience_start_date"])[:10]
                except:
                    resultado += "," + "Null"
                try:
                    resultado += "," + (application["experience_end_date"])[:10]
                except:
                    resultado += "," + "Null"
                try:
                    resultado += "," + application["status"]
                except:
                    resultado += "," + "Null"
                try:
                    resultado += "," + application["current_status"]
                except:
                    resultado += "," + "Null"
                try:
                    resultado += "," + (application["created_at"])[:10]
                except:
                    resultado += "," + "Null"
                try:
                    resultado += "," + (application["updated_at"])[:10]
                except:
                    resultado += "," + "Null"
                try:
                    resultado += "," + application["person"]["full_name"]
                except:
                    resultado += "," + "Null"
                try:
                    resultado += "," + application["person"]["dob"]
                except:
                    resultado += "," + "Null"
                try:
                    resultado += "," + (application["person"]["home_lc"])["name"]
                except:
                    resultado += "," + "Null"
                try:
                    resultado += "," + (application["person"]["home_lc"])["country"]
                except:
                    resultado += "," + "Null"
                resultado += "\n"
    return resultado
                
#funcion que regresa la info de una opp
def oppInfo(idopp):
    r = requests.get("https://gis-api.aiesec.org/v2/opportunities/"+str(idopp)+"?access_token=" + token) 
    content =  r.content
    data = json.loads(content)
    resultado = ""
    resultado += "," + ((data["branch"]["name"]).replace(",","-"))
    resultado += "," + (data["created_at"])[:10]
    resultado += "," + ((data["title"]).replace(",","-"))
    resultado += "," + data["host_lc"]["name"]
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