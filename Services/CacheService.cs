using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Channels;
using Microsoft.VisualBasic.FileIO;

namespace Services;

public class CacheService
{
    private readonly FTSService _ftsService;

    public CacheService(FTSService ftsService)
    {
        _ftsService = ftsService;
        ProcessQueue();
    }

    private Channel<KeyValuePair<string, string>> _channel = Channel.CreateUnbounded<
        KeyValuePair<string, string>
    >();

    public async Task Enqueue(string key, string value)
    {
        await _channel.Writer.WriteAsync(new KeyValuePair<string, string>(key, value));
    }

    public async Task<KeyValuePair<string, string>> Dequeue()
    {
        return await _channel.Reader.ReadAsync();
    }

    public List<string> Search(string searchText)
    {
        var foundValues = _ftsService.Search(searchText);
        return foundValues is null ? new List<string>() : foundValues;
    }

    // write a method that starts a background task to process the queue
    public void ProcessQueue()
    {
        Task.Run(async () =>
        {
            while (await _channel.Reader.WaitToReadAsync())
            {
                while (_channel.Reader.TryRead(out var item))
                {
                    Console.WriteLine($"Key: {item.Key}, Value: {item.Value}");
                    _ftsService.AddRange(new List<KeyValuePair<string, string>> { item });
                }
            }
        });
    }
}
