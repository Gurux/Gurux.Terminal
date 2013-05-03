See An [Gurux](http://www.gurux.org/ "Gurux") for an overview.

Join the Gurux Community or follow [@Gurux](https://twitter.com/guruxorg "@Gurux") for project updates.

Open Source GXTerminal media component, made by Gurux Ltd, is a part of GXMedias set of media components, which programming interfaces help you implement communication using modem communication. 

Our media components also support the following connection types: network and Serial Port.

For more info check out [Gurux](http://www.gurux.org/ "Gurux").

We are updating documentation on Gurux web page. 

If you have problems you can ask your questions in Gurux [Forum](http://www.gurux.org/forum).

Simple example
=========================== 
Before use you must set following settings:
* PhoneNumber
* PortName
* BaudRate
* DataBits
* Parity
* StopBits

It is also good to listen following events:
* OnError
* OnReceived
* OnMediaStateChange


```csharp

GXTerminal cl = new GXTerminal();
cl.PhoneNumber = "Phone number";
cl.PortName = "COM1";
cl.BaudRate = 9600;
cl.DataBits = 8;
cl.Parity = System.IO.Ports.Parity.None;
cl.StopBits = System.IO.Ports.StopBits.One;
cl.Open();

```

Data is send with send command:

```csharp
cl.Send("Hello World!");
```
In default mode received data is coming as asynchronously from OnReceived event.

```csharp
cl.OnReceived += new ReceivedEventHandler(this.OnReceived);

```
Data can be send as syncronous if needed:

```csharp
lock (cl.Synchronous)
{
    string reply = "";
    ReceiveParameters<string> p = new ReceiveParameters<string>()
    //ReceiveParameters<byte[]> p = new ReceiveParameters<byte[]>()
    {
       //Wait time tells how long data is waited.
       WaitTime = 1000,
       //Eop tells End Of Packet charachter.
       Eop = '\r'
    };
    cl.Send("Hello World!", null);
    if (gxNet1.Receive(p))
    {
	reply = Convert.ToString(p.Reply);
    }
}
```