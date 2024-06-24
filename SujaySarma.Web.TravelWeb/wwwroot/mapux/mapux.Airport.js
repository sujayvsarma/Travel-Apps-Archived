/*
    sujaytravelapps.com
    © 2019, Sujay V Sarma. All rights reserved.
    Product : sujaytravelapps.com
    About this file: This script manages all the client-side interactions of the MapUX view.

    We are using JSDoc to perform type hinting for VS IDE
*/

/**
 * An airport
 */
class Airport {

    /**
     * Construct a new airport
     * @param {string} name - Name of airport
     * @param {string} iata - IATA code of airport
     * @param {string} city - City airport is in
     * @param {string} country - Country airport is in
     * @param {string} isoCountry - ISO Country code
     * @param {number} lat - Latitude coordinates of airport
     * @param {number} lon - Longitude coordinates of airport
     */
    constructor(name, iata, city, country, isoCountry, lat, lon) {
        this.Name = name;
        this.Country = country;
        this.ISOCountry = isoCountry;
        this.City = city;
        this.Iata = iata;
        this.Lat = lat;
        this.Lon = lon;
    }

    get Name() { return this._name; }
    set Name(name) { this._name = name; }

    get Iata() { return this._iata; }
    set Iata(iata) { this._iata = iata; }

    get City() { return this._city; }
    set City(city) { this._city = city; }

    get Country() { return this._country; }
    set Country(country) { this._country = country; }

    get ISOCountry() { return this._isoCountry; }
    set ISOCountry(iso_country) { this._isoCountry = iso_country; }

    get Lat() { return this._lat; }
    set Lat(lat) { this._lat = lat; }

    get Lon() { return this._lon; }
    set Lon(lon) { this._lon = lon; }
}