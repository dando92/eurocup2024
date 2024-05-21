# Eurocup 2024
Esistono 5 entità:
 - ITGMania: Gioco che produce gli score non formattati in json (Ad oggi)
 - ScoreProducer: riceve gli score tramite udp da itgmania e li formatta come json da inviare al tournament manager. Questo passacarte non manterrà alcuna info riguardo il torneo poichè verrà, in futuro, sostituito da ITGMania.
 -  TournamentManager: Mantiene il file db,json che è lo stato corrente del torneo e ne aggiorna le info sulla base degli score ricevuti. Inoltre riceverà delle richieste per la creazione dei vari match. 
 - TournamentGenerator: Permette di aggiungere match ed eventuali altri info necessarie al tournament manager.
 - TournamentViewer: Richiede al manager il db.json e ne visuali# Introduzione

Vengono predisposti tutte le divisioni, fasi e round direttamente nel db.json prima di iniziare il torneo.
Esempio divsioni: Lower, Mid, Upper, Hard Tech, Stamina etc
Esempio Fase: Primo turno, Secondo turno, Quarti, seminifinale, finale, etc
Esempio Match: Girone A, Girone B, Girone C etc
Esempio Round: canzone1 Girone A, canzone2 Girone A, etc

# NewRound
Il frontend TournamentGenerator, una volta selezionati divisione, fase e match ti permette di inserire un nuovo round composto da una lista di giocatori e una lista di canzoni da giocare.

l'inserimento di queste informazioni invia un json al TournamentManager così composto:
```json
{
  "matchId": 1,
  "players": ["player1", "player2"],
  "songs":   ["canzone1", "canzone2"],
}
```
Alla ricezione di questo messaggio il TournamentManager va a generare all'interno del db.json un nuovo round per ogni canzone nel match corrispondente.
```json
{
  "id": 1,
  "name": "Girone A",
  "players": ["player1", "player2"],
  "rounds": [
    {
      "id": 1,
      "songName": "canzone1",
      "standings": []
    },
        {
      "id": 1,
      "songName": "canzone2",
      "standings": []
    }
  ]
}
```
# SetActiveMatch
Invia un json con l'id del match attivo per fare l'associazione degli score nella sezione coretta.
```json
{
  "matchId": 1
}
```

# ITGMania sends a score to ScoreProducer
Quando lo score producer riceve un messaggio via udp da ITGMania effettua le seguenti operazioni: Parsifica la stringa ricevuta trasformandola in json. Salva una copia locale json e invia gli stessi dati ad un foglio google su drive. lo score prodotto in json verrà inviato tramite socket alla porta 8080 con il seguente formato:
```json
TBD
```

> **Porte su cui vengono ricevuti e prodotti i dati:**
> 
> SYNCSTART_UDP_PORT = 53000; 
> 
> WEBSOCKET_PORT = 8080;

# Score Update to TournamentManager from ScoreProduer
Il Tournament manager sulla porta 8080 riceve l'update degli score che verranno salvati nel db.json.

Se non c'è un match attivo, gli score vengono scartati.

Lo score contiene il nome della canzone e il player che l'ha prodotto. Se la coppia di chiavi non viene trovata nel match corrente, lo score viene scartato. In caso di esito positivo vengono aggiunti agli standings della canzone corrispondente lo score di ogni player. Ogni volta che viene aggiunto uno score ad un round, viene fatto il calcolo del numero di standings. Se è uguale al numero di player nel match, ordina gli elementi per score e inserisce il numero di punti guadagnati ad ogni score.

> **Notifica push di update al viewer???????**

```json
{
  "id": 1,
  "name": "Girone A",
  "players": ["player1", "player2"],
  "songs":   ["canzone1", "canzone2"],
  "rounds": [
    {
      "id": 1,
      "songName": "canzone1",
      "standings": [
        {
          "player": "player1",
          "score": 10,
          "percentage": 100
        },
        {
          "player": "player2",
          "score": 5,
          "percentage": 50
        }
      ]
    },
    {
      "id": 2,
      "songName": "canzone2",
      "standings": []
    }
  ]
}
```
