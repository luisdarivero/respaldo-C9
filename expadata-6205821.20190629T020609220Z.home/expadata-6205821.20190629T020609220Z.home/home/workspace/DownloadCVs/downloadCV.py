import urllib2

def main():
    with open("MKT Analyst") as f:
        content = f.readlines()
        content = [x.strip() for x in content] 
        name = "file "
        count = 1
        for x in content:
            download_file(x, name + str(count))
            count += 1

def download_file(download_url, name):
    response = urllib2.urlopen(download_url)
    file = open(name + ".pdf", 'w')
    file.write(response.read())
    file.close()
    print("Completed")

if __name__ == "__main__":
    main()