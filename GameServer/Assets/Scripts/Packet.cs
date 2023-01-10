using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

//this code is used to read and write different data types into packets
//it was adapted from code found in this video https://www.youtube.com/watch?v=uh8XaC0Y5MA by Tom Weiland

//packets that server will receive 
public enum PacketsIn
{
    shakeReceived = 1,
    playerUpdate
}

//packet types
public enum PacketsOut
{
    handshake = 1,
    spawnPlayer,
    despawnPlayer,
    playerPosition,
    playerRotation
}



public class Packet : IDisposable //forces dispose() method for releasing unmanaged resources
{

    private int readPosition;
    private List<byte> buffer;
    private byte[] bufferReadable;
    

    //empty packet
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

    //packet with id
    public Packet(int thisId)
    {
        buffer = new List<byte>(); 
        readPosition = 0; 

        Write(thisId); 
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
            readPosition -= 4; 
        }
    }




    //byte array read + write
    public void Write(byte[] value)
    {
        buffer.AddRange(value);
    }

    
    public byte[] ReadBytes(int length, bool changeReaderPosition = true)
    {
        if (buffer.Count > readPosition)
        {
            
            byte[] value = buffer.GetRange(readPosition, length).ToArray(); 
            if (changeReaderPosition)
            {
               
                readPosition += length; 
            }
            return value; 
        }
        else
        {
            throw new Exception("Error reading value - array of bytes");
        }
    }


    //int read + write
    public void Write(int value)
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    
    public int ReadInt(bool changeReaderPosition = true)
    {
        if (buffer.Count > readPosition)
        {
            //check for unread bytes
            int value = BitConverter.ToInt32(bufferReadable, readPosition); 
            if (changeReaderPosition)
            {
                readPosition += 4; 
            }
            return value; 
        }
        else
        {
            throw new Exception("Error reading value - int");
        }
    }

    //float read + write
    public void Write(float value)
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    
    public float ReadFloat(bool changeReaderPosition = true)
    {
        if (buffer.Count > readPosition)
        {
            //check for unread bytes
            float value = BitConverter.ToSingle(bufferReadable, readPosition); 
            if (changeReaderPosition)
            {
                
                readPosition += 4; 
            }
            return value; 
        }
        else
        {
            throw new Exception("Error reading value - float");
        }
    }




    //string read + write
    public void Write(string value)
    {
        Write(value.Length); 
        buffer.AddRange(Encoding.ASCII.GetBytes(value)); 
    }

    
    public string ReadString(bool changeReaderPosition = true)
    {
        try
        {
            int length = ReadInt(); 
            string value = Encoding.ASCII.GetString(bufferReadable, readPosition, length); 
            if (changeReaderPosition && value.Length > 0)
            {
                
                readPosition += length; 
            }
            return value; 
        }
        catch
        {
            throw new Exception("Error reading value str");
        }
    }







    //vector3 read + write
    //this is used for postion and velocity data
    public void Write(Vector3 value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
    }

    
    public Vector3 ReadVector3(bool changeReaderPosition = true)
    {
        return new Vector3(ReadFloat(changeReaderPosition), ReadFloat(changeReaderPosition), ReadFloat(changeReaderPosition));
    }




    //this is used to make sure the player starts with the desired locked rotation 
    public void Write(Quaternion value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
        Write(value.w);
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