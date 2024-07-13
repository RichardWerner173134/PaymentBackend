# Funktionalitäten
## Usermanagement
- Erfassung aller Nutzer per Id (frei wählbar)
	- `get /users`
	- `get /users/{username}`
		
## Erfassung von Bezahlungen

- Erfassen einer Rechnung [€]
	- Wer hat bezahlt?
	- Wer hat es erfasst?
	- Was wurde bezahlt?
	- Für wen wurde bezahlt?
	- Wieviel wurde bezahlt?
	- Wann fand die Zahlung statt?
	- Wann wurde die Zahlung erfasst?
	- `post /payments`

## Abfragen der Ergebnisse
- Welche Bezahlungen gibt es?
	- `get /payments`
	- `get /payments/{paymentId}`

- Was habe ich bezahlt?
	- Auflistung aller Bezahlungen, deren Creditor ich bin
		- `get /payments-for-creditor/{username}`
	- Summe der Ausgaben (Wieviel habe ich insgesamt bezahlt?) 
	- Summe der Ausgaben ohne meinen Anteil (Was habe ich für andere bezahlt?)
		- `get /payment-overview-for-creditor/{username}`
- Was wurde für mich bezahlt?
	- Auflistung aller Bezahlungen, deren Debitor ich bin
		- `get /payments-for-debitor/{username}`
	- Summe der Ausgaben mit meinem Anteil (Wieviel wurde für mich bezahlt?)
		- `get /payment-overview-for-debitor/{username}`
- Wem schulde ich etwas? Wer schuldet mir etwas?
	- Auflistung aller anteiligen Interaktionen zwischen mir und anderen Nutzern
		- 
- Wie ist meine Balance

