# Audio Players {#manual-audio-player}
Audio Player System is created so that designers can setup and create complex sounds with minimal coding input. Audio Players are composable pieces of code that user can setup from Inspector in-place, Scriptable object assets or via separate metadata of AudioClipAssets. To extend functionality of AudioPlayers System one writes new class derived from AudioPlayer (for example refer to code of Jenga.ClipAudioPlayer). Audio Players are not separate audio system, they
are built on top of AudioSources.

#### Usage from code
To use AudioPlayers one only needs to add serialized property to your MonoBehaviour, or any serializable via Unity class, with correct attributes
and call one of its public function when you need to use it:
~~~~~~~~~~~~~{.cs}
using UnityEngine;
using Jenga;

public class TestAudioPlayers : MonoBehaviour {

    [SerializeReference, TypeMenu]
    public AudioPlayer testPlayer = new(); 

    public void SomeTestEvent() {
        testPlayer.PlayAt(transform.position);
    }
}
~~~~~~~~~~~~~

One can also built requred AudioPlayer entierly from code; it is convenient
to use curly-braced initializers for this:
~~~~~~~~~~~~~{.cs}
var player = new PlaylistAudioPlayer() {
    playbackMode = PlaylistAudioPlayer.PlaybackMode.PlayAll,
    sequenceMode = PlaylistAudioPlayer.SequenceMode.InOrder,

    players = new AudioPlayer[] {
        new ClipAudioPlayer() { clip = myClip1 },
        new SilenceAudioPlayer() { seconds = .2f },
        new ClipAudioPlayer() { clip = myClip2 }
    }
};

player.PlayAt(transform.position);
~~~~~~~~~~~~~

#### Usage in Inspector with TypeMenu's 
You can use jenga's type menu to create Audio Player entirely in inspector.
Player, analogues to one in previous section, will look like this in inspector: 

![Audio Player setup in inspector](/Images/AudioPlayers-ExampleLooks.jpg){html:width=40%}

To create this use dropdown menus on righthand side to select required players:

![Audio Player Type Menu](/Images/AudioPlayers-ExampleTypeMenu.jpg){html:width=20%}

You can also use right-click menu to get some advanced functionality:

![Type Menu Advanced functionality](/Images/TypeMenus-AdvancedFunctionality.jpg){html:width=20%}

#### Move player to asset
If you want to setup complex audio player and then use it multiple times you probably should consider creating specific AudioPlayer Asset:

![AudioPlayer Asset](/Images/AudioPlayers-CreateAsset.jpg){html:width=80%}

One sets up AudioPlayer asset in the same manner. Use AssetAudioPlayer (or just Asset in type-menu) to play AudioPlayer asset.

#### Use Metadata
Audio Metadata is AudioPlayer asset created for any AudioClip Asset you need to.
If you use ClipAudioPlayer and clip has such metadata, metadata will be played instead. All metadata contained in single asset called Metadata Master: 

![Metadata Master](/Images/AudioPlayers-MetadataMaster.jpg){html:width=40%}

To add, remove and edit metadata one uses Audio Metadata Editor Window:

![Audio Metadata Editor window path](/Images/AudioPlayers-AudioMetadataEditor.jpg){html:width=50%}

Once window open (and metadata master selected in it), select AudioClip, click on Create button and setup metadata you need:

![Audio Metadata Editor Usage](/Images/AudioPlayers-AudioMetadataEditorUsage.jpg){html:width=80%}

*Be careful! Dont use ClipAudioPlayer inside metadata, as it will trigger infinite recursion. Use ClipIgnoreMetadataAudioPlayer instead.*

Main use case for metadata is to set up mixer group, change volume, modify sound before usage in game.

#### Hints
- Use Jenga.AudioModificatorPlayer to modify volume instead of Jenga.WithSettingsAudioPlayer if you want to make sound louder. They also stack with each other, compared to Settings, that rewrite.
- Check out Jenga.AmbianceZone if you need one.

#### See Also
- Jenga.AudioPlayer
- Jenga.PlaylistAudioPlayer
- Jenga.ClipAudioPlayer
- Jenga.ClipIgnoreMetadataAudioPlayer
- Jenga.SilenceAudioPlayer
- Jenga.AudioModificatorPlayer
- Jenga.WithSettingsAudioPlayer
- Jenga.AmbianceZone