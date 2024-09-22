import _ from "lodash";

function objectAssign<T>(data: any, Idata: any){
    const newData = {};
    for (const key in Idata) {
        if (_.isObject(data)){
            newData[Idata[key]] = data[Idata[key]];
        }
    }

    return newData as T;
}

export {
    objectAssign
};