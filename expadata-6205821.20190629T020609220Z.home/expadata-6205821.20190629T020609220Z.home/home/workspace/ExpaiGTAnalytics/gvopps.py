
import requests
import json

token = "5e6e73c10d7b6bd51bc8589de55fe00c2fcb186a656e18bf3e9fe0ecde6f761f"

#funcion que regresa el numero de paginas necesarias
def numberOfPages(idOpp):
    url = "https://gis-api.aiesec.org/v2/opportunities/"+idOpp+"/applications?access_token=" + token
    r = requests.get(url) 
    content =  r.content
    data = json.loads(content)
    return int(json.dumps(data["paging"]["total_pages"]))

#consulta todos los aplicantes de una opp
def getPeople(oppId, numberOfPages):
    resultado = ""
    lista = []
    for x in range(1, numberOfPages + 1):
        url = "https://gis-api.aiesec.org/v2/opportunities/"+oppId+"/applications?access_token="+token+"&page="+str(x)
        r = requests.get(url) 
        content =  r.content
        data = json.loads(content)
        #ciclo que analiza cada aplicion por separado
        
        resultado = ""
        for application in (data["data"]):
            app = application["current_status"]
            if(app != "open" and app != "rejected" and app != "withdrawn"):
                
                try:
                    resultado += application["person"]["full_name"]
                except:
                    resultado +=  "Null"
                try:
                    resultado += "," +(application["experience_start_date"])[:10]
                except:
                    resultado += "," +"Null"
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
                
                lista.append(resultado)
                resultado = ""
    return lista


#regresa una acion
def action(nombre):
    
    if nombre=="opportunities":
        return "https://gis-api.aiesec.org/v2/opportunities?access_token="+token+"&filters[committee]=1582"
    else: 
        return "Error"
        
#funcion que imprime un json
def printJ(j):
    jsonToString = json.dumps(j)
    print jsonToString
    
#funcion que obtiene el numero de paginas para ver todas las oportunidades
def getPageOppNumber(j):
    return int(j["paging"]["total_pages"])
    
#funcion que obtiene la informacion de una opp
def getOppInfo(j):
    #printJ(j)
    resultado = ""
    linea = ""
    currentOpp = 0;
    for oportunidad in (j["data"]):
        if(str(oportunidad["programmes"]["short_name"]) == "GV"):
            try:
                linea += str(oportunidad["id"])#id de la opp
                currentOpp = str(oportunidad["id"])
            except:
                linea += "Null"
            try:
                linea += "," + (oportunidad["title"]).replace(",","-")#titulo de opp
            except:
                linea += ",Null"
            linea += getMoreOppInfo(currentOpp)
            try:
                linea += "," + oportunidad["office"]["name"]
            except:
                linea += ",Null"
            try:
                linea += "," + oportunidad["status"]
            except:
                linea += ",Null"
            try:
                linea += "," + oportunidad["current_status"]
            except:
                linea += ",Null"
            try:
                linea += "," + str(oportunidad["duration"])
            except:
                linea += ",Null"
            try:
                linea += "," + (oportunidad["earliest_start_date"])[:10]
            except:
                linea += ",Null"
            try:
                linea += "," + (oportunidad["latest_end_date"])[:10]
            except:
                linea += ",Null"
            try:
                linea += "," + (oportunidad["created_at"])[:10]
            except:
                linea += ",Null"
            #acaba primera parte
            
            #resultado += linea + "\n"
            #se anaden los eps de esta opp
            gente = getPeople(str(oportunidad["id"]),numberOfPages(str(oportunidad["id"])))
            for x in gente:
                resultado += x +","+ linea + "\n"
            
            linea = "";
            
    return resultado
    
#funcion que regresa datos de una oportunidad
def getMoreOppInfo(op):
    request = requests.get("https://gis-api.aiesec.org/v2/opportunities/"+op+"?access_token=" + token) 
    pageContent = request.content
    jsonData = json.loads(pageContent)
    resultado = ""
    
    try:
        resultado += "," + (jsonData["branch"]["name"]).replace(",","-")
    except:
        resultado += ",Null"
   
    return resultado
#main del programa
def main():
    #try:
        #pidiendo el json
        r = requests.get(action("opportunities")) 
        content =  r.content
        data = json.loads(content)
        paginas = getPageOppNumber(data) #imprime el nuero total de paginas
        resultadoFinal = "" #donde se guardan todos los resultados
        resultadoFinal += "Full name, RE date, finish date, EP status, EP current status, EP day of apply, EP date of birth, Home LC, Home country, Opp ID, Opp tittle,NGO, Host LC, Opp status, Opp current status, Opp duration, Opp earliest start day, Opp latest end date, Date Opp created\n"
        #lista de encabezados
        encabezados = ["id opp","title","url","status","current_status","Product","applications_count","duration","applications_close_date","earliest_start_date","latest_end_date","LC","created_at"]
        for pagina in range(1,paginas+1):
            print pagina
            request = requests.get("https://gis-api.aiesec.org/v2/opportunities?access_token="+token+"&page="+str(pagina)+"&filters[committee]=1582") 
            pageContent = request.content
            jsonData = json.loads(pageContent)
            #printJ(jsonData)
            resultadoFinal += getOppInfo(jsonData)
            text_file = open("OutputGVopps.txt", "w")
            text_file.write(resultadoFinal.encode('utf-8').strip())
            text_file.close()
            #print getOppInfo(jsonData["data"][0])
            #print resultadoFinal
        #printJ (data)
        
        print resultadoFinal
        text_file = open("OutputGVopps.txt", "w")
        text_file.write(resultadoFinal.encode('utf-8').strip())
        text_file.close()
    #except:
        #print("Error de ejecucion" + sys.exc_info()[0])
#&page=
#llamando a la funcion principal
main()
