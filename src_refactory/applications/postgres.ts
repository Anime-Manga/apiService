import pg from "pg";
const {Pool} = pg;

import Logger from "./logger";
const logger = new Logger("pg");
 
let pool: pg.Pool;

let inRestartingPool = false;

//settings
let address: string, port: number, username: string, password: string, nameDatabase: string;

async function init(newAddress: string = "localhost", newPort: number = 5432, newUsername: string = null, newPassword: string = null, newNameDatabase: string = "anime&manga"){
    address = newAddress;
    port = newPort;
    username = newUsername;
    password = newPassword;
    nameDatabase = newNameDatabase;

    await retryPool();

    pool.on('error', async (err) => {
        logger.error("[POOL] Disconnected to progresql, details:", err);

        if(!inRestartingPool){
            await retryPool();
        }
    })
}

function createPool(){
    logger.debug("Creating Pool...");

    //setup configuration
    pool = new Pool({
        database: nameDatabase,
        host: address,
        port,
        user: username,
        password,
        connectionTimeoutMillis: 5000
    });

    logger.debug("Created Pool");
}

async function retryPool(){
    inRestartingPool = true;

    do{
        try{
            createPool();
            break;
        }catch(err){
            logger.error('Failed connect to postgres, details:', err);
        }
    }while(true);

    inRestartingPool = false;
    logger.info("Connected to postgres!")
}

export default pool;

export {
    init
}