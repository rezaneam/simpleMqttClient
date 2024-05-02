import { useState, useEffect } from 'react'
import { sensorReading } from './sensorReading'
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from '@microsoft/signalr';
import './App.css'

const signalrUrl = "http://localhost:5153/hub"
const attachSignal = (
  hub: HubConnection,
  name: string,
  callback: (...args: any[]) => any,
) => {
  if (hub) hub.on(name, callback);
  else console.error('Hub is not initialized');
};



function App() {
  const [count, setCount] = useState(0)
  const [receivedMessages, AddMessageNo] = useState(0)
  const [hub, setHub] = useState<HubConnection | null>(null);

  useEffect(() => {
    hub?.stop();

    const hubConnection = new HubConnectionBuilder()
      .withUrl(`${signalrUrl}`)
      .withAutomaticReconnect()
      .configureLogging(import.meta.env.VITE_IS_LOCAL === 'true' ? LogLevel.Debug : LogLevel.Warning)
      .build();

    const connect = async () => {
      try {
        await hubConnection
          .start()
          .then(() => console.log('Connected to SignalR endpoint'));
      } catch (err) {
        console.error('error while connecting: ', err);
      }
    };

    const init = async () => {
      hubConnection.onclose(async () => {
          await connect();
      });

      hubConnection.onreconnecting(() => {
        console.log('Reconnecting to signalR endpoint');
      });
      hubConnection.onreconnected(() => {
        console.log('Reconnected to signalR endpoint');
      });

      await connect();
      attachSignal(hubConnection, "Reading", (newData: sensorReading) => {
        console.log("Reading: ", newData);
        AddMessageNo((receivedMessages) => receivedMessages + 1)
      });

      setHub(hubConnection);
      console.log('SignalR, ', `Connecting to ${signalrUrl}`);
    };

    
    init();
    return () => {
      hubConnection.off("DoNothing");
      hubConnection.stop();
    };
  }, []);

  return (
    <>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>

        <h3>
          Number of recieved Mesasages: {receivedMessages}
        </h3>
      </div>
    </>
  )
}

export default App
