
import requests
import json

token = "8947c4e54832136f9134a4547225121b3ac6f5ac86136f215ed54aceea488c6a"

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
        try:
            linea += str(oportunidad["id"])#id de la opp
            currentOpp = str(oportunidad["id"])
        except:
            linea += "Null"
        try:
            linea += "," + (oportunidad["title"]).replace(",","-")#titulo de opp
        except:
            linea += ",Null"
        try:
            linea += "," + oportunidad["url"]
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
            linea += "," + oportunidad["programmes"]["short_name"]
        except:
            linea += ",Null"
        try:
            linea += "," + str(oportunidad["applications_count"])
        except:
            linea += ",Null"
        try:
            linea += "," + str(oportunidad["duration"])
        except:
            linea += ",Null"
        try:
            linea += "," + (oportunidad["applications_close_date"])[:10]
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
            linea += "," + oportunidad["office"]["name"]
        except:
            linea += ",Null"
        try:
            linea += "," + (oportunidad["created_at"])[:10]
        except:
            linea += ",Null"
        #acaba primera parte
        linea += getMoreOppInfo(currentOpp)
        resultado += linea + "\n"
        linea = "";
        
    return resultado
    
#funcion que regresa datos de una oportunidad
def getMoreOppInfo(op):
    request = requests.get("https://gis-api.aiesec.org/v2/opportunities/"+op+"?access_token=" + token) 
    pageContent = request.content
    jsonData = json.loads(pageContent)
    resultado = ""
    
    try:
        resultado += "," + jsonData["sub_product"]["name"] 
    except:
        resultado += ",Null"
    try:
        resultado += "," + (jsonData["branch"]["name"]).replace(",","-")
    except:
        resultado += ",Null"
    try:
        resultado += "," + str(jsonData["openings"])
    except:
        resultado += ",Null"
    try:
        resultado += "," + str(jsonData["available_openings"])
    except:
        resultado += ",Null"
    try:
        resultado += "," + (jsonData["specifics_info"]["salary"]).replace(",",".")
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
        #lista de encabezados
        encabezados = ["id opp","title","url","status","current_status","Product","applications_count","duration","applications_close_date","earliest_start_date","latest_end_date","LC","created_at"]
        for pagina in range(1,paginas+1):
            print pagina
            request = requests.get("https://gis-api.aiesec.org/v2/opportunities?access_token="+token+"&page="+str(pagina)+"&filters[committee]=1582") 
            pageContent = request.content
            jsonData = json.loads(pageContent)
            #printJ(jsonData)
            resultadoFinal += getOppInfo(jsonData)
            text_file = open("Output.txt", "w")
            text_file.write(resultadoFinal.encode('utf-8').strip())
            text_file.close()
            #print getOppInfo(jsonData["data"][0])
            #print resultadoFinal
        #printJ (data)
        
        print resultadoFinal
        text_file = open("Output.txt", "w")
        text_file.write(resultadoFinal.encode('utf-8').strip())
        text_file.close()
    #except:
        #print("Error de ejecucion" + sys.exc_info()[0])
#&page=
#llamando a la funcion principal
main()
