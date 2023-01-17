# DemoMinimalAPI
 Demonstration of a minimal API with database connection
 
## To Run Api

    1. Install docker and WSL2
 
 - https://www.docker.com/products/docker-desktop/
 - https://learn.microsoft.com/pt-br/windows/wsl/install
 -
 
    2. With docker running, open de command prompt inside the folder of project ~/DemoMinimalAPI and run command "docker-compose up" to create docker image
	
![image](https://user-images.githubusercontent.com/117870158/212793919-0341e1e6-9036-44dc-badd-40cbb218a425.png)
 
    3. With Visual Studio's Package Manager run the command "update-database -Context MinimalContextDb"    
	
![image](https://user-images.githubusercontent.com/117870158/212794037-68f55979-46d7-49e1-a402-997c0ad22620.png)

	4. With Visual Studio's Package Manager run the command "update-database -Context NetDevPackAppDbContext"
	
![image](https://user-images.githubusercontent.com/117870158/212794127-b7acbba1-61e3-4392-8977-e0e53ea847ba.png)

	5. Run API
