/*
    flighthoteltravelideas.com
    © 2019, Sujay V Sarma. All rights reserved.
    Product : flighthoteltravelideas.com
    About this file: Defines a Coordinate type

    We are using JSDoc to perform type hinting for VS IDE
*/

/**
 * Lat/Lon coordinate system
 */
class Coordinate {
    /**
     * Initialize coordinates
     * @param {number} lat - Latitude
     * @param {number} lon - Longitude
     */
    constructor(lat, lon) {
        this._lat = lat;
        this._lon = lon;
    }

    /**
     * Get the latitude value
     */
    get Latitude() {
        return this._lat;
    }

    /**
     * Set the latitude value
     * @param {number} lat - Latitude
     */
    set Latitude(lat) {
        this._lat = lat;
    }

    /**
     * Get the longitude value
     */
    get Longitude() {
        return this._lon;
    }

    /**
     * Set the longitude value
     * @param {number} lon - Longitude
     */
    set Longitude(lon) {
        this._lon = lon;
    }
}
