<b>NeoAxis Engine</b> is an integrated development environment with built-in 3D and 2D game engine.

Royalty-free, open source platform.

<a href="https://www.neoaxis.com/">www.neoaxis.com</a>

<div class="image" align="center"><a href="https://www.neoaxis.com/images/2020_3/NeoAxis_2020_3.png"><img src="https://www.neoaxis.com/images/2020_3/NeoAxis_2020_3.jpg" alt="" width="1000" vspace="10"></a></div>

<h2>Video review</h2>

[![Video review](https://img.youtube.com/vi/YxZbaEWkegY/0.jpg)](https://www.youtube.com/watch?v=YxZbaEWkegY)

<h2>Supported platforms by the NeoAxis editor</h2>
<ul>
<li>Windows</li>
<li>Mac (coming soon)</li>
</ul>

<h2>Supported platforms</h2>
<ul>
<li>Windows</li>
<li>UWP (coming soon in sources, already supported in binaries from neoaxis.com)</li>
<li>Android (coming soon)</li>
<li>...</li>
</ul>

<h2>Building</h2>
<ul>
<li>NeoAxis.Managed.sln - Restore NuGet packets before compilation. Visual Studio 2017+.</li>
<li>By default is better to use the Release configuration by performance reasons, debugging is supported.</li>
<li>Download Sci-fi Demo and Nature Demo from the Asset Store window.</li>
</ul>

<h2>Programming tips</h2>
<ul>
<li>Set up NeoAxis.Editor assembly as StartUp project to make run with debugger.</li>
<li>Add a new code to the NeoAxis.CoreExtension by default. Project assembly is also good.</li>
<li>Use English code page for your project if it possible: https://github.com/NeoAxis/NeoAxisEngine/issues/1</li>
</ul>

<h2>License</h2>

Shortly about the license. You can use NeoAxis Engine for free without any future royalties and distribute source codes subject to the following conditions:
<ul>
<li>1. When publishing your product, you must add "Made with NeoAxis Engine (<span>www</span>.neoaxis.com)" with the product information and in its credits.
</li>

<li>2. You can distribute the NeoAxis editor with your product. When you do it the window title of the editor must be:
  
  "Your project name - NeoAxis Engine 2020.3 (<span>www</span>.neoaxis.com)".
  
  Or if you made engine modification:
  
  "Your project name - Modified version of NeoAxis Engine 2020.3 (<span>www</span>.neoaxis.com)"

This can be done by changing the value of the ModifiedVersionOfNeoAxisEngine field in the Sources\Engine\NeoAxis.Core\Utility\EngineInfo.cs file.</li>

<li>3. You can distribute the source code of the NeoAxis Engine. In the case of publishing any part of the modified source code, for example, by means of creating a fork or other distribution method, you automatically become the contributor of the NeoAxis Engine. This means that the NeoAxis Group Ltd reserves the right to use your modified source code at its discretion, for example, to improve the original version of the NeoAxis Engine. Copyright for modified code is saved to you.</li>
</ul>

www.neoaxis.com/LICENSE.txt

<h2>Frequently Asked Questions</h2>
<ul>
<li><b>Did I understand correctly that the NeoAxis Engine is provided free of charge with full source code, while the no any royalties? I should not even add splash screen when starting my game, just mention it in the credits?</b>
That's right. However, we will be glad of any help with the development of the engine, including its popularization.</li>
<li><b>What is the difference between the version on the GitHub and the installer from www.neoaxis.com?</b>
It is the same. The GitHub version has slightly more recent updates. Although you will need to additionally download Sci-fi Demo and Nature Demo inside the editor from Asset Store window if you want to look at them.</li>
</ul>
