## 🧮Api Service
Questo progetto verrà utilizzato per esporre i dati in maniera facile e veloce con il database postgresql.

### Expose Ports:
- 80 tcp

### Information general:
- `not` require volume mounted on Docker
### Variabili globali richiesti:
```sh
example:
    #--- DB ---
    DATABASE_CONNECTION: User ID=guest;Password=guest;Host=localhost;Port=33333;Database=db; [require]
    
    #--- Rabbit ---
    USERNAME_RABBIT: "guest" #guest [default]
    PASSWORD_RABBIT: "guest" #guest [default]
    ADDRESS_RABBIT: "localhost" #localhost [default]

    #--- API ---
    PORT_API: "33333" #5000 [default]
    
    #--- Logger ---
    LOG_LEVEL: "Debug|Info|Error" #Info [default]
    WEBHOOK_DISCORD_DEBUG: "url" [not require]
    
    #--- General ---
    BASE_PATH: "/folder/anime" or "D:\\\\Directory\Anime" #/ [default]
    LIMIT_THREAD_PARALLEL: "8" #5 [default]
```