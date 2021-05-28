* Replaced the FIX Repository based dictionary with one generated from the FIX Orchestra.
	* Fields with with multi character enumerated values are now fully supported.
	* FIX.4.2, FIX.4.4, and FIX.5.0SP2 (FixLatest) are now the only included orchestrations.
		* Others can be added but these are the versions now supported by fixtrading.org.
	* Data dictionary customisation for HKEX and ITG message and field types has been removed.
* The messages view data dictionary panel now displays the complete pedigree for each message, field, and value.
* Removed the smart paste options from the messages view paste form.
* The default SenderCompID and TargetCompID for a new session is now INITIATOR/ACCEPTOR.