Web extractor
=============

Task 1 Web Extraction
---------------------

Write a data extractor that reads out content from extraction.booking.html and extracts the below listed information. The attached screen capture (screencapture-www-booking-com.png) explains the colors below:

*   Hotel name (red)
*   Address (red)
*   Classification / stars (red)
*   Review points (pink)
*   Number of reviews (pink)
*   Description (blue)
*   Room categories (green)
*   Alternative hotels (yellow)

> Task 1 is implemented using html forms, Task 2 using jQuery Ajax.
> 
> Note: Frankly, I didn't implement validations, status codes, proper response objects, json serialization namings, exception handling, and blockUI functionality to save time...

Task 2 Reporting
----------------

Generate an excel report based on raw data (hotelrates.json) that has a similar structure to the screen capture (screencapture-excel-report.png). LOS = length of stay (number of room nights) The column BREAKFAST\_INCLUDED is a Boolean variable (1: breakfast included, 0: breakfast excluded).

> Task 2 is implemented using jQuery Ajax to call an API instad of traditional form posting.
