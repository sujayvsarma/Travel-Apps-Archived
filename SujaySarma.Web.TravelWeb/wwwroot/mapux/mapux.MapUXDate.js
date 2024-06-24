/*
    sujaytravelapps.com
    © 2019, Sujay V Sarma. All rights reserved.
    Product : sujaytravelapps.com
    About this file: This script defines the MapUXDate type

    We are using JSDoc to perform type hinting for VS IDE
*/

/**
 * Emulates the behavior of the .NET DateTime object, but we dont use the time portion (always returned as zeroes)
 */
class MapUXDate {

    /**
     * Create a date from a string or a Javascript Date object
     * @param {string|Date|MapUXDate} dt - Date as a string or Javascript Date object. Format is of two types: 'MMM dd, yyyy' and 'yyyy-MM-ddZHH:mm:ss'
     */
    constructor(dt) {
        if (dt === null) {
            this._date = new Date();
        }
        else if (typeof dt === 'string') {
            if (dt.indexOf(',') > 0) {
                var dt_comp = dt.split(' ');
                var zdt = dt_comp[2] + '-';

                switch (dt_comp[0]) {
                    case 'Jan': zdt += '01'; break;
                    case 'Feb': zdt += '02'; break;
                    case 'Mar': zdt += '03'; break;
                    case 'Apr': zdt += '04'; break;
                    case 'May': zdt += '05'; break;
                    case 'Jun': zdt += '06'; break;
                    case 'Jul': zdt += '07'; break;
                    case 'Aug': zdt += '08'; break;
                    case 'Sep': zdt += '09'; break;
                    case 'Oct': zdt += '10'; break;
                    case 'Nov': zdt += '11'; break;
                    case 'Dec': zdt += '12'; break;
                }

                zdt += '-' + dt_comp[1].substring(0, 2) + 'T00:00:00';

                this._date = new Date(zdt);
            }
            else if ((dt.indexOf('T') > 0) || (dt.indexOf('Z') > 0)) {
                this._date = new Date(dt);
            }
            else {
                throw '[' + dt + '] is not a recognized form of date';
            }

        }
        else if ((typeof dt === 'object') && (dt instanceof Date)) {
            this._date = new Date(dt.getFullYear(), dt.getMonth(), dt.getDate(), 0, 0, 0, 0);
        }
        else if ((typeof dt === 'object') && (dt instanceof MapUXDate)) {
            // Month-1 because js Date is 0-indexed!
            this._date = new Date(dt.Year, dt.Month - 1, dt.Day, 0, 0, 0, 0);
        }
        else {
            throw '[' + dt.toString() + '] is not a recognized form of date';
        }
    }

    /**
     * Get the day of month
     */
    get Day() {
        return this._date.getDate();
    }

    /**
     * Set the day of month
     */
    set Day(day) {
        this._date.setDate(day);
    }

    /**
     * Get the month
     */
    get Month() {
        return this._date.getMonth() + 1;
    }

    /**
     * Set the day of month (1-based like in .NET !!!)
     */
    set Month(month) {
        this._date.setMonth(month - 1);
    }

    /**
     * Get the year
     */
    get Year() {
        return this._date.getFullYear();
    }

    /**
     * Get the year
     */
    set Year(year) {
        this._date.setFullYear(year);
    }

    /**
     * Gets the name of the month (3-letter version)
     */
    get MonthName() {
        var names = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        return names[this.Month - 1];
    }

    /**
     * Add a number of days to the date. The original date is changed!
     * @param {number} days
     * @returns {MapUXDate} - The new date
     */
    AddDays(days) {
        this._date.setDate(this._date.getDate() + days);
        return this;
    }

    /**
     * Returns a universal format date string ('yyyy-MM-ddZHH:mm:ss')
     * @returns {string} - String in format 'yyyy-MM-ddZHH:mm:ss'
     */
    ToStringUniversalFormat() {
        return this.Year + '-' + this._getTwoDigitNumber(this.Month) + '-' + this._getTwoDigitNumber(this.Day) + 'Z00:00:00';
    }

    /**
     * Returns a UX format date string ('MMM dd, yyyy')
     * @returns {string} - String in format 'MMM dd, yyyy'
     */
    ToStringUX() {
        return this.MonthName + ' ' + this._getTwoDigitNumber(this.Day) + ', ' + this.Year;
    }

    /**
     * Returns the string in a form that .NET's DateTime class can parse
     */
    ToStringNetFx() {
        return this.Year + '-' + this._getTwoDigitNumber(this.Month) + '-' + this._getTwoDigitNumber(this.Day) + 'T00:00:00';
    }

    /**
     * The string-rep for the object. This would be mostly be called by JS itself for marshalling, so we use the UFmt.
     * @returns {string} - String in format 'yyyy-MM-ddZHH:mm:ss'
     */
    toString() {
        return this.ToStringUniversalFormat();
    }

    /**
     * Pads a number to two digits and returns the string
     * @param {number} num
     * @returns {string} - the padded number string
     */
    _getTwoDigitNumber(num) {
        var v = '';
        if (num < 10) {
            v = '0';
        }

        v += num.toString();
        return v;
    }
}
