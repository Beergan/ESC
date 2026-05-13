// In the following line, you should include the prefixes of implementations you want to test.
window.indexedDB = window.indexedDB || window.mozIndexedDB || window.webkitIndexedDB || window.msIndexedDB;
// DON'T use "var indexedDB = ..." if you're not in a function.
// Moreover, you may need references to some window.IDB* objects:
window.IDBTransaction = window.IDBTransaction || window.webkitIDBTransaction || window.msIDBTransaction || { READ_WRITE: "readwrite" }; // This line should only be needed if it is needed to support the object's constants for older browsers
window.IDBKeyRange = window.IDBKeyRange || window.webkitIDBKeyRange || window.msIDBKeyRange;
// (Mozilla has never prefixed these objects, so we don't need window.mozIDB*)

(function () {
    'use strict';
    window.localdbstore = {
        openDb: async (dbName, version, stores) => {

            return new Promise(function (resolve, reject) {
                var request = indexedDB.open(dbName, version);

                request.onerror = function (event) {
                    console.log("Why didn't you allow my web app to use IndexedDB?!");
                    reject(false)
                };

                request.onsuccess = function (event) {
                    window.db = event.target.result;
                    resolve(true);
                };

                // This event is only implemented in recent browsers   
                request.onupgradeneeded = function (event) {
                    // Save the IDBDatabase interface 
                    window.db = event.target.result;
                    stores.forEach((store) => {
                        window.db.createObjectStore(store.name, { keyPath: store.key });
                    });
                         
                    resolve(true);
                };
            });   
        },
        addItem: async (store, data) => {
            return new Promise(function (resolve, reject) {

                var request = window.db.transaction(store, "readwrite")
                    .objectStore(store)
                    .add(data);

                request.onsuccess = function (event) {
                    resolve(true);
                };

                request.onerror = function (event) {
                    resolve(false);
                }
            });
        },
        deleteItem: async (store, id) => {
            return new Promise(function (resolve, reject) {

                var request = window.db.transaction(store, "readwrite")
                    .objectStore(store)
                    .delete(id);

                request.onsuccess = function (event) {
                    resolve(true);
                };

                request.onerror = function (event) {
                    resolve(false);
                }
            });
        },
        getAll: async (store) => {
            return new Promise(function (resolve, reject) {

                var transaction = db.transaction(store, 'readonly');
                var objectStore = transaction.objectStore(store);

                if ('getAll' in objectStore) {
                    objectStore.getAll().onsuccess = function (event) {
                        resolve(event.target.result)
                    };
                } else {
                    var results = [];
                    objectStore.openCursor().onsuccess = function (event) {
                        var cursor = event.target.result;
                        if (cursor) {
                            results.push(cursor.value);
                            cursor.continue();
                        } else {
                            resolve(event.target.result)
                        }
                    };
                }
            });
        },
        getItem: async (store, id) => {
            return new Promise(function (resolve, reject) {

                var request = window.db.transaction(store, "readwrite")
                    .objectStore(store)
                    .get(id);

                request.onsuccess = function (event) {
                    resolve(true);
                };

                request.onerror = function (event) {
                    resolve(false);
                }
            });
        },
        invokeAsPromise: (promiseHandler, method, args) => {
            console.log(args);

            let promise = null;
            switch (args.length) {
                case 1:
                    promise = window.localdbstore[method](args[0]);
                    break;
                case 2:
                    promise = window.localdbstore[method](args[0], args[1]);
                    break;
                case 3:
                    promise = window.localdbstore[method](args[0], args[1], args[2]);
                    break;
                case 4:
                    promise = window.localdbstore[method](args[0], args[1], args[2], args[3]);
                    break;
                case 5:
                    promise = window.localdbstore[method](args[0], args[1], args[2], args[3], args[4]);
                    break;
                case 6:
                    promise = window.localdbstore[method](args[0], args[1], args[2], args[3], args[4], args[5]);
                    break;
                case 7:
                    promise = window.localdbstore[method](args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                    break;
                default:
                    promise = window.localdbstore[method]();
            }

            // Process the promise
            promise.then((value) => {
                promiseHandler.invokeMethodAsync('SetResult', value);
            }).catch((error) => {
                if (error === undefined) error = "Something bad happened";
                console.log(error);
                promiseHandler.invokeMethodAsync('SetError', error);
            });
        }
    };
})();