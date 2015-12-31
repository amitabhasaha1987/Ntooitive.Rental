function getMiles(distanceinKm) {
    return (distanceinKm * 0.621371).toFixed(2);
}
Storage.prototype.setObj = function (key, obj) {
    return this.setItem(key, JSON.stringify(obj));
}
Storage.prototype.getObj = function (key) {
    return JSON.parse(this.getItem(key));
}