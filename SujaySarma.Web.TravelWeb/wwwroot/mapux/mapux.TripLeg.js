/*
    sujaytravelapps.com
    © 2019, Sujay V Sarma. All rights reserved.
    Product : sujaytravelapps.com
    About this file: This script manages all the client-side interactions of the MapUX view.

    We are using JSDoc to perform type hinting for VS IDE
*/

/**
 * A leg in the itinerary
 */
class TripLeg {

    /**
     * Create a new trip leg
     * @param {number} id - Id of the trip
     * @param {Airport} origin - Airport of origin
     * @param {Airport} destination - Airport of destination
     * @param {MapUXDate} departure - Date of departure
     * @param {string} classOfTravel - Class of travel ("economy","premiumeconomy","business","first")
     */
    constructor(id, origin, destination, departure, classOfTravel) {
        this.Id = id;
        this.Origin = origin;
        this.Destination = destination;
        this.Departure = departure;
        this.Passengers = {
            Adults: 1,
            Children: 0,
            Infants: 0
        };
        this.ClassOfTravel = classOfTravel;
    }

    get Id() { return this._id; }
    set Id(id) { this._id = id; }

    get Origin() { return this._origin; }
    set Origin(airport) { this._origin = airport; }

    get Destination() { return this._destination; }
    set Destination(airport) { this._destination = airport; }

    get Departure() { return this._departure; }
    set Departure(date) { this._departure = date; }

    get Passengers() { return this._passengers; }
    set Passengers(pax) { this._passengers = pax; }

    get ClassOfTravel() { return this._classOfTravel; }
    set ClassOfTravel(cot) { this._classOfTravel = cot; }

    /**
     * Set passenger count
     * @param {number} adults - Number of adults
     * @param {number} children - Number of children
     * @param {number} infants - Number of infants
     */
    SetPassengers(adults, children, infants) {
        this.Passengers = {
            Adults: adults,
            Children: children,
            Infants: infants
        };
    }

    /**
     * Create a URL-encoded string for form-submission
     */
    ToFormString() {
        return 'leg' + this._id.toString() + '=' +
            this._origin.Iata + ',' + this._origin.ISOCountry + '|' + this._destination.Iata + ',' + this._destination.ISOCountry + '|' +
            this._departure.ToStringNetFx() + '|' +
            this._passengers.Adults + ',' + this._passengers.Children + ',' + this._passengers.Infants + '|' +
            this._classOfTravel;
    }
}