import { createHash } from "node:crypto"

function encryptPassword(password: string){
    return createHash("sha256").update(password).digest("hex");
}

export default {
    encryptPassword
}