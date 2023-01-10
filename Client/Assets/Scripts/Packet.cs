using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

//this code is used to read and write different data types into packets
//it was adapted from code found in this video https://www.youtube.com/watch?v=uh8XaC0Y5MA by Tom Weiland


//packet types that the player will receive from the server
public enum PacketsIn
{
    handshake = 1,
    spawnPlayer,
    despawnPlayer,
    otherPlayerPosition,
    otherPlayerRotation
}

//packets that the client will send to the server
public enum PacketsOut
{
    shakeReceived = 1,
    playerUpdate
    
}

public class Packet : IDisposable //forces dispose() method for releasing unmanaged resources
{

    private int readPosition;
    private List<byte> buffer;
    private byte[] bufferReadable;
    

    //empty packet creation
    public Packet()
    {
        buffer = new List<byte>(); 
        readPosition = 0; 
    }

    //packet for receiving data
    public Packet(byte[] data)
    {
        buffer = new List<byte>();
        readPosition = 0;

        SetByteArray(data);
    }

    //creates packet with a packet id
    public Packet(int id)
    {
        buffer = new List<byte>(); 
        readPosition = 0; 

        Write(id); 
    }

       
    public void SetByteArray(byte[] data)
    {
        //prepare packet to be read
        Write(data);
        bufferReadable = buffer.ToArray();
    }

    
    public void WritePacketLength()
    {
        //length of packet at start of packet 
        buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count)); 
    }

    
    public void InsertInt(int value)
    {
        //put int at start of packet data
        buffer.InsertRange(0, BitConverter.GetBytes(value)); 
    }

    
    public byte[] ToArray()
    {
        //convert packet to array
        bufferReadable = buffer.ToArray();
        return bufferReadable;
    }

    
    public int Length()
    {
        return buffer.Count; 
    }

   
    public int UnreadLength()
    {
        return Length() - readPosition; 
    }

    
    public void Reset(bool doReset = true)
    {
        if (doReset)
        {
            buffer.Clear(); 
            bufferReadable = null;
            readPosition = 0; 
        }
        else
        {
            readPosition -= 4; // "Unread" the last read int
        }
    }
    

   
    
    //read and write arrays of bytes 
    public void Write(byte[] value)
    {
        buffer.AddRange(value);
    }

    public byte[] ReadByteArray(int length, bool changeReaderPosition = true)
    {
        if (buffer.Count > readPosition)
        {
            
            byte[] value = buffer.GetRange(readPosition, length).ToArray(); 
            if (changeReaderPosition)
            {
                // If changeReaderPosition is true
                readPosition += length; 
            }
            return value; 
        }
        else
        {
            throw new Exception("Error reading value - array of bytes");
        }
    }







    //read and write ints
    public void Write(int value)
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    public int ReadInt(bool changeReaderPosition = true)
    {
        if (buffer.Count > readPosition)
        {
            // If there are unread bytes
            int value = BitConverter.ToInt32(bufferReadable, readPosition); // Convert the bytes to an int
            if (changeReaderPosition)
            {
                // If changeReaderPosition is true
                readPosition += 4; // Increase readPosition by 4
            }
            return value; // Return the int
        }
        else
        {
            throw new Exception("Error reading value - int");
        }
    }




    //read and write floats
    public void Write(float value)
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }



    public float ReadFloat(bool changeReaderPosition = true)
    {
        if (buffer.Count > readPosition)
        {
            // If there are unread bytes
            float value = BitConverter.ToSingle(bufferReadable, readPosition); // Convert the bytes to a float
            if (changeReaderPosition)
            {
                // If changeReaderPosition is true
                readPosition += 4; // Increase readPosition by 4
            }
            return value; // Return the float
        }
        else
        {
            throw new Exception("Error reading value - float");
        }
    }






    //read and write strings
    
    public void Write(string value)
    {
        Write(value.Length); // Add the length of the string to the packet
        buffer.AddRange(Encoding.ASCII.GetBytes(value)); // Add the string itself
    }

    
    public string ReadString(bool changeReaderPosition = true)
    {
        try
        {
            int length = ReadInt(); // Get the length of the string
            string value = Encoding.ASCII.GetString(bufferReadable, readPosition, length); // Convert the bytes to a string
            if (changeReaderPosition && value.Length > 0)
            {
                // If changeReaderPosition is true string is not empty
                readPosition += length; // Increase readPosition by the length of the string
            }
            return value; // Return the string
        }
        catch
        {
            throw new Exception("Error reading value str");
        }
    }





    
    public void Write(Vector3 value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
    }
    
             

    //player position & velo
    public Vector3 ReadVector3(bool changeReaderPosition = true)
    {
        return new Vector3(ReadFloat(changeReaderPosition), ReadFloat(changeReaderPosition), ReadFloat(changeReaderPosition));
    }

    //read player roatation (old)
    public Quaternion ReadQuaternion(bool changeReaderPosition = true)
    {
        return new Quaternion(ReadFloat(changeReaderPosition), ReadFloat(changeReaderPosition), ReadFloat(changeReaderPosition), ReadFloat(changeReaderPosition));
    }


    private bool isDisposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
                buffer = null;
                bufferReadable = null;
                readPosition = 0;
            }

            isDisposed = true;
        }
    }

    public void Dispose()
    {
        //cleanup
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}