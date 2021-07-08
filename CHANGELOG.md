* Replaced the FIX Repository based dictionary with one generated from the FIX Orchestra.
	* Fields with with multi character enumerated values are now fully supported.
	* FIX.4.0, FIX.4.2, FIX.4.4, and FIX.5.0SP2 (FixLatest) are now the only included orchestrations.
		* FIX.4.2, FIX.4.4, and FIX.5.0SP2 are the only versions now supported by fixtrading.org.
		* FIX.4.0 has been generated in order to support custom ITG/Virtu message types.
	* Data dictionary customisation for HKEX message and field types has been removed.
* The messages view data dictionary panel now displays the complete pedigree for each message, field, and value.
* Removed the smart paste options from the messages view paste form.
* The default SenderCompID and TargetCompID for a new session is now INITIATOR/ACCEPTOR.
* The format of the session *.filters file has been changed.
	* Existing filters will be ignored and the file will be rewritten.
*	The libraries FIX Client is built upon Fix, Fix.Dictionary, and Fix.Common are now available from nuget.
	* The nuget packages are named Geh.Fix, Geh.Fix.Dictionary, and Geh.Fix.Common to make then unique in the repository.
* The history view now has a data dictionary inspector like the messages view.
* Fixed a null reference exception that would intermittently cause crashes.