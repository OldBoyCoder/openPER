# openPER

openPer is a version of the ePer system used to hold parts information for vehicles manufactured by the Fiat group.

Caveat
------

This code was based on examination of the database and data files underlying release 84 of ePer.  Anyone using openPer to purchase parts
or perform maintenance on their vehicle does so at their own risk and the authors of openPer should not be held liable for any losses incurred.

Introduction
------------

ePer has many fine features but it is old software and there is a risk that future versions of Windows may make it difficult to run.
The openPer project was started to create an open source alternative that uses the original Fiat data but in a front-end that can be amended
as necessary.

Not all of the ePer features have been replicated, we've not looked at prices lists for instance, but we have attempted to add as many
ePer features as possible.  The key ones are:

- Hierarchical access to parts diagrams for all vehicles covered by release 84 of ePer
- The ability to search by vehicle VIN to retrieve vehicle specific information
- The ability to select a particular version of a vehicle
- Once a specific vehicle or version has been selected parts diagrams are filtered to show those that are appropriate 
- The ability to search by Part number and see which vehicles and diagrams used that part
- The ability to launch a new tab with a Google search run for a particular part number
- Some multi-language support.  The descriptions of parts are retrieved in the appropriate language.  The UI is not yet fully multi-lingual
- Show all modifications made to a vehicle over time

Nomenclature
------------

There is a hierarchy within ePer which goes like this Make->Model->Catalogue->Category->Subcategory->Table->Drawing->Part->Cliche

Taking these in turn:

- Make, self explanatory. Note that within the code there are two levels of make as the high level 'F' for Fiat is split into commercial and non-commercial vehicles
- Model, the high level name for a vehicle, an example being 'Panda'
- Catalogue, a sub-division within Model.  Model can be split into different catalogues based on date such as 'Panda MY 2011 (2010-2011)' or 
sometimes other features such as transmission where these would cause the diagrams to vary greatly.
- Category, a high level split of the components of the vehicle. So 'Clutch', 'Gearbox', 'Steering' and so on.  It isn't always obvious 
which category a part will be found in but experience will gradually help.
- Sub-category, a further division of the components.  'Fuel injection system' has subcategories such as 'Fuel Tank', 'Air cleaner and ducts' and 'Exhaust Manifold'.
- Table, a lowest level grouping of drawings within a sub-category, sometimes necessary where a component is very complex or changed radically over time
- Drawing, an actual parts diagram along with a list of parts for that diagram.  A part on a diagram may change over time and so may have multiple part numbers along with information about when it changed
- Part, hopefully self-explanatory
- Cliche, there are times when an individual part on one diagram can be further decomposed.  A diagram showing the parts making up the part is clalled a cliche.  Cliches are often shared between vehicles, brake calipers are a common example






