import CryptoJS from "crypto-js";

function hashPassword(password: string){
    return CryptoJS.SHA256(password).toString();
}

export {
    hashPassword
};