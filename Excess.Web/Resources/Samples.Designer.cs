﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Excess.Web.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Samples {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Samples() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Excess.Web.Resources.Samples", typeof(Samples).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to class Arrays 
        ///{
        ///    void main()
        ///    {
        ///        var arr = [10, 15, 30];
        ///        
        ///        foreach(int val in arr)
        ///            console.write(val);
        ///    }
        ///}.
        /// </summary>
        internal static string Arrays {
            get {
                return ResourceManager.GetString("Arrays", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to environment
        ///    .keyword(&quot;asynch&quot;)
        ///    .keyword(&quot;synch&quot;);
        ///
        ///syntax
        ///    .extension(&quot;asynch&quot;, ExtensionKind.Code, ProcessAsynch)
        ///    .extension(&quot;synch&quot;, ExtensionKind.Code, ProcessSynch);.
        /// </summary>
        internal static string AsynchExtension {
            get {
                return ResourceManager.GetString("AsynchExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to private static SyntaxNode ProcessAsynch(SyntaxNode node, Scope scope, SyntacticalExtension&lt;SyntaxNode&gt; extension)
        ///{
        ///    if (extension.Kind == ExtensionKind.Code)
        ///    {
        ///        var result = AsynchTemplate
        ///            .ReplaceNodes(AsynchTemplate
        ///                .DescendantNodes()
        ///                .OfType&lt;BlockSyntax&gt;(), 
        ///                (oldNode, newNode) =&gt; extension.Body);
        ///
        ///        var document = scope.GetDocument&lt;SyntaxToken, SyntaxNode, SemanticModel&gt;();
        ///        document.change(node.Parent, Ros [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string AsynchTransform {
            get {
                return ResourceManager.GetString("AsynchTransform", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to void main()
        ///{
        ///    asynch()
        ///    {
        ///        var x = expensive();
        ///          	
        ///        synch()
        ///        {
        ///            notify(x);
        ///        }
        ///    }
        ///}.
        /// </summary>
        internal static string AsynchUsage {
            get {
                return ResourceManager.GetString("AsynchUsage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to var shop = spawn&lt;barbershop&gt;();
        ///var barber1 = spawn&lt;barber&gt;();
        ///var barber2 = spawn&lt;barber&gt;();
        ///
        ///start(barber1);
        ///start(barber2);
        ///start(shop, barber1, barber2);
        ///
        ///var rand = new Random();
        ///for (int i = 1; i &lt;= 30; i++)
        ///{
        ///    Thread.Sleep((int)(3000*rand.NextDouble()));
        ///    shop.visit(i);
        ///}.
        /// </summary>
        internal static string BarbersApp {
            get {
                return ResourceManager.GetString("BarbersApp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to concurrent class barber
        ///{
        ///    [Forever]                        
        ///    void main()
        ///    {
        ///        {shave -&gt; tip}
        ///    }
        ///
        ///    public void shave(int client)
        ///    {
        ///        await wait(sec: rand(1, 2));
        ///    }
        ///
        ///    double _tip = 0;
        ///    public void tip(double amount)
        ///    {
        ///        _tip += amount;
        ///        console.write(&quot;tipped: &quot; + amount.ToString(&quot;C2&quot;) + &quot; total: &quot; + _tip.ToString(&quot;C2&quot;));
        ///    }
        ///}.
        /// </summary>
        internal static string BarbersBarber {
            get {
                return ResourceManager.GetString("BarbersBarber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to concurrent class barbershop
        ///{
        ///    barber _barber1, _barber2;
        ///    bool _busy1, _busy2;
        ///                    
        ///    void main(barber b1, barber b2)
        ///    {
        ///        _barber1 = b1; _busy1 = false;
        ///        _barber2 = b2; _busy2 = false;
        ///
        ///        while(true)
        ///            await visit;
        ///    }
        ///
        ///    [Blocking]
        ///    public void visit(int client)
        ///    {
        ///        console.write(&quot;entered : &quot; + client);
        ///        if (_busy1 &amp;&amp; _busy2)
        ///            await next_client;
        ///
        ///        if (!_busy1)
        ///        {
        ///            _bu [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string BarbersShop {
            get {
                return ResourceManager.GetString("BarbersShop", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to concurrent class chopstick
        ///{
        ///    [Forever]
        ///    void main()
        ///    {
        ///        {acquire | release}
        ///    }
        ///
        ///    public void acquire(object owner)
        ///    {
        ///        if (_owner != null)
        ///        {
        ///            await release;
        ///        }
        ///                                
        ///        _owner = owner;
        ///    }
        ///	
        ///    public void release(object owner)
        ///    {
        ///        if (_owner != owner)
        ///            throw new InvalidOperationException();
        ///
        ///        _owner = null;
        ///    }
        ///
        ///    private object _owner;
        ///}.
        /// </summary>
        internal static string Chopsticks {
            get {
                return ResourceManager.GetString("Chopsticks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to environment.
        ///    keyword(&quot;contract&quot;);
        ///
        ///syntax
        ///    .extension(&quot;contract&quot;, ExtensionKind.Code, ProcessContract);.
        /// </summary>
        internal static string ContractExtension {
            get {
                return ResourceManager.GetString("ContractExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to private static SyntaxNode ProcessContract(SyntaxNode node, Scope scope, SyntacticalExtension&lt;SyntaxNode&gt; extension)
        ///{
        ///    if (extension.Kind == ExtensionKind.Code)
        ///    {
        ///        var block = extension.Body as BlockSyntax;
        ///
        ///        List&lt;StatementSyntax&gt; checks = new List&lt;StatementSyntax&gt;();
        ///        foreach (var st in block.Statements)
        ///        {
        ///            var stExpression = st as ExpressionStatementSyntax;
        ///            if (stExpression == null)
        ///            {
        ///                scope.AddError(&quot;contrac [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ContractTransform {
            get {
                return ResourceManager.GetString("ContractTransform", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to void main(int x, int y)
        ///{
        ///    contract()
        ///    {
        ///        x &gt; 10;
        ///        x &lt; 20;
        ///        x &gt;= y;
        ///    }
        ///}.
        /// </summary>
        internal static string ContractUsage {
            get {
                return ResourceManager.GetString("ContractUsage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to namespace Events 
        ///{
        ///    class Base
        ///    {
        ///        //c# way
        ///        public delegate void FinishedHandler(int x, int y);
        ///        public event FinishedHandler Finished;
        ///    }
        ///  
        ///    class Derived : Base
        ///    {
        ///        public event closed(int x, int y);
        ///
        ///        on closed(x, y)
        ///        {
        ///            console.write(x);
        ///        }
        ///
        ///        on Finished()
        ///        {
        ///            console.write(y);
        ///        }
        ///    }
        ///}.
        /// </summary>
        internal static string Events {
            get {
                return ResourceManager.GetString("Events", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to class Extensions
        ///{
        ///    void main()
        ///    {
        ///        asynch()
        ///        {
        ///            var x = expensive();
        ///          	
        ///            synch()
        ///            {
        ///              	notify(x);
        ///            }
        ///        }
        ///    }
        ///}.
        /// </summary>
        internal static string Extensions {
            get {
                return ResourceManager.GetString("Extensions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to concurrent class factorial
        ///{
        ///    static int buckets = 3;
        ///    double main(int n)
        ///    {
        ///        double fact = 0;
        ///        {fact = parallel&lt;double&gt;(n, 
        ///            map: () =&gt; {
        ///                var size = (int)(n/buckets);
        ///                for(int i = 0; i &lt; buckets; i++)
        ///                {
        ///                    int lower = i == 0? 1 : i * size + 1;
        ///                    int upper = i == (buckets - 1)? n : (i + 1) * size;
        ///
        ///                    yield return () =&gt; {
        ///                        double result =  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Factorial {
            get {
                return ResourceManager.GetString("Factorial", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to var fact = spawn&lt;factorial&gt;();
        ///
        ///var n = 100;
        ///var result = start&lt;double&gt;(fact, n);
        ///
        ///double naive = 1.0;
        ///for (int i = 1; i &lt;= n; i++)
        ///    naive *= i;
        ///
        ///console.write(&quot;Distributed: &quot; + result);
        ///console.write(&quot;Naive: &quot; + naive);.
        /// </summary>
        internal static string FactorialApp {
            get {
                return ResourceManager.GetString("FactorialApp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to class Functions 
        ///{
        ///    void main()
        ///    {
        ///        //as javascript-style argument
        ///        foo(function(value) {
        ///            console.write(value);
        ///            return value + 5;
        ///        });
        ///    }
        ///  
        ///    //as a method, with type inference
        ///    function foo(function&lt;int, int&gt; callback) //as a type
        ///    {
        ///        int result = callback(73);
        ///        return result;
        ///    }
        ///
        ///    string function bar()
        ///    {
        ///        return &quot;bar&quot;;
        ///    }
        ///}.
        /// </summary>
        internal static string Functions {
            get {
                return ResourceManager.GetString("Functions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to void main()
        ///{
        ///    console.write(&quot;Hello World&quot;);
        ///}.
        /// </summary>
        internal static string HelloWorld {
            get {
                return ResourceManager.GetString("HelloWorld", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to public class Lolcat
        ///{
        ///    constructor(string name)
        ///    {
        ///        Name = name;
        ///        initSpeekers();
        ///    }
        ///
        ///    string property Name;
        ///
        ///    method Say(Mood mood)
        ///    { 
        ///        Speek speek = speekers[mood](mood);
        ///        console.write(Name + &quot;: &quot; + speek.Say);
        ///        return speek.Mood;
        ///    }
        ///
        ///    typedef Func&lt;Mood, Speek&gt; 		  Speeker; 
        ///    typedef Dictionary&lt;Mood, Speeker&gt; Speekers; 
        ///        
        ///    protected Speekers speekers = new Speekers();
        ///
        ///    protected virtual void initSpeekers()
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string LolCats {
            get {
                return ResourceManager.GetString("LolCats", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to var cats = [
        ///    new Lolcat(&quot;Kitty&quot;), 
        ///    new Lolcat(&quot;Furry Kitty&quot;), 
        ///    new Trollcat(&quot;Ball of Fur&quot;)
        ///];
        ///
        ///var mood = Mood.Nize;
        ///
        ///for(int i = 0; i &lt; 30; i++)
        ///{
        ///    int cat = random.Int(3);
        ///    mood = cats[cat].Say(mood);
        ///}
        ///
        ///console.write(&quot;Powered by speaklolcat.com&quot;);.
        /// </summary>
        internal static string LolCatsApplication {
            get {
                return ResourceManager.GetString("LolCatsApplication", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to public enum Mood
        ///{
        ///    Nize,
        ///    Hungry,
        ///    Skerd,
        ///}
        ///
        ///public class Speek
        ///{
        ///    public string Say  { get; set; }
        ///    public Mood   Mood { get; set; }
        ///}
        ///
        ///public class Speeks
        ///{
        ///    static method AnySpeek(Mood mood)
        ///    {
        ///        Speek[] choices;
        ///        switch (mood)
        ///        {
        ///            case Mood.Nize: choices = NizeTalk; break;
        ///            case Mood.Skerd: choices = SkerdTalk; break;
        ///            default: choices = HungryTalk; break;
        ///        }
        ///        
        ///        return choices[random. [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string LolCatsSpeek {
            get {
                return ResourceManager.GetString("LolCatsSpeek", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to public class Trollcat : Lolcat
        ///{
        ///    public Trollcat(string name)
        ///        : base(name)
        ///    {
        ///    }
        ///
        ///    protected override void initSpeekers()
        ///    {
        ///        base.initSpeekers();
        ///
        ///        speekers[Mood.Nize] = function(mood)
        ///        {
        ///            return new Speek { Say = &quot;BAD KITTEH!&quot;, Mood = Mood.Skerd };
        ///        };
        ///
        ///        speekers[Mood.Hungry] = hungry;
        ///    }
        ///
        ///    function hungry(Mood m)
        ///    {
        ///        return new Speek { Say = &quot;NO CHEEZBURGER!&quot;, Mood = Mood.Skerd };
        ///    }
        ///}.
        /// </summary>
        internal static string LolCatsTrollcat {
            get {
                return ResourceManager.GetString("LolCatsTrollcat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to environment
        ///    .keyword(&quot;match&quot;);
        ///
        ///lexical
        ///    .match()
        ///        .token(&quot;match&quot;, named: &quot;keyword&quot;)
        ///        .enclosed(&apos;(&apos;, &apos;)&apos;)
        ///        .token(&apos;{&apos;)
        ///        .then(lexical.Transform()
        ///            .replace(&quot;keyword&quot;, &quot;switch&quot;)
        ///            .then(ProcessMatch, referenceToken: &quot;keyword&quot;));.
        /// </summary>
        internal static string MatchExtension {
            get {
                return ResourceManager.GetString("MatchExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to private static SyntaxNode ProcessMatch(SyntaxNode node, Scope scope)
        ///{
        ///    var switchExpr = node as SwitchStatementSyntax;
        ///    if (switchExpr == null)
        ///    {
        ///        scope.AddError(&quot;match01&quot;, &quot;malformed match&quot;, node);
        ///        return node;
        ///    }
        ///
        ///    //store items to simplify
        ///    var cases = new List&lt;ExpressionSyntax&gt;();
        ///    var statements = new List&lt;StatementSyntax&gt;();
        ///    var defaultStatement = null as StatementSyntax;
        ///
        ///    foreach (var section in switchExpr.Sections)
        ///    {
        ///        bool isDe [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string MatchTransform {
            get {
                return ResourceManager.GetString("MatchTransform", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to void main()
        ///{
        ///    int y = 20;
        ///    match (x)
        ///    {
        ///        case 0:
        ///        {
        ///            y = 20;
        ///            return null;
        ///        }
        ///        case &gt; 10: 
        ///        	return 30;
        ///        case y &gt; 10: 
        ///        	return 40;
        ///        default: 
        ///        	return x;
        ///    }
        ///}.
        /// </summary>
        internal static string MatchUsage {
            get {
                return ResourceManager.GetString("MatchUsage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to class Misc 
        ///{
        ///    typedef IEnumerable&lt;int&gt; 	 Data;
        ///    typedef function&lt;int, int&gt;   Calculation;
        ///    typedef Dictionary&lt;int, int&gt; Results;
        ///
        ///    void main()
        ///    {
        ///        var data = [2, 3, 5, 7, 11];
        ///        var calc = function(int value)
        ///        {
        ///            return value*5;
        ///        };
        ///        
        ///        Results results = new Results();
        ///        calculate(data, calc, results);
        ///    }
        ///
        ///    private void calculate(Data data, Calculation fn, Results results)
        ///    {
        ///        foreach(var d in data)
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Misc {
            get {
                return ResourceManager.GetString("Misc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to concurrent class philosopher 
        ///{
        ///    [Forever]
        ///    void main(string name, chopstick left, chopstick right) 
        ///    {
        ///        _name  = name;
        ///        _left  = left;
        ///        _right = right;
        ///                               
        ///        {think()}
        ///    }
        ///	
        ///    void think()
        ///    {
        ///        console.write(_name + &quot; is thinking&quot;);
        ///        await wait(sec: rand(1.0, 2.0))
        ///                        -&gt; hungry();
        ///    }
        ///
        ///    void hungry()
        ///    {
        ///        console.write(_name + &quot; is hungry&quot;);
        ///
        ///        await (_left.acq [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Philosophers {
            get {
                return ResourceManager.GetString("Philosophers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to var names = [&quot;Kant&quot;, &quot;Archimedes&quot;, &quot;Nietzche&quot;, &quot;Plato&quot;, &quot;Engels&quot;];
        ///var count = names.Length;
        ///
        ///var philosophers = spawn&lt;philosopher&gt;(count).ToArray();
        ///var chopsticks   = spawn&lt;chopstick&gt;(count).ToArray();
        ///
        ///foreach (var chopstick in chopsticks)
        ///    start(chopstick);
        ///
        ///for (var i = 0; i &lt; count; i++)
        ///{
        ///    var left = chopsticks[i];
        ///    var right = i == count - 1 ? chopsticks[0] : chopsticks[i + 1];
        ///    start(philosophers[i], names[i], left, right);
        ///}.
        /// </summary>
        internal static string PhilosophersApp {
            get {
                return ResourceManager.GetString("PhilosophersApp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to var resource = spawn&lt;resource&gt;();
        ///var clients = spawn&lt;client&gt;(5);
        ///
        ///start(resource);
        ///foreach(var client in clients)
        ///    start(client, resource);.
        /// </summary>
        internal static string ReadersWriterApp {
            get {
                return ResourceManager.GetString("ReadersWriterApp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to concurrent class client
        ///{
        ///    resource _resource;
        ///    void main(resource r)
        ///    {
        ///        _resource = r;
        ///        {read() &amp; write()}
        ///    }
        ///
        ///    [Forever]
        ///    public void read()
        ///    {
        ///        await wait(sec: rand(0.2, 0.5));
        ///        var result = 0;
        ///        {result = _resource.read()}
        ///
        ///        console.write(&quot;read: &quot; + result);
        ///    }
        ///
        ///    [Forever]
        ///    public void write()
        ///    {
        ///        await wait(sec: rand(1, 2));
        ///        _resource.write();
        ///    }
        ///}.
        /// </summary>
        internal static string ReadersWriterClient {
            get {
                return ResourceManager.GetString("ReadersWriterClient", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to concurrent class resource
        ///{
        ///    [Blocking]
        ///    public int read()
        ///    {
        ///        if (_writing)
        ///            await writing_done;
        ///                        
        ///        return _value++;        
        ///    }
        ///
        ///    bool _writing = false;
        ///    int _value = 0;                    
        ///
        ///    [Blocking]
        ///    public void write()
        ///    {
        ///        console.write(&quot;writing at: &quot; + _value);
        ///        _writing = true;
        ///        await wait(sec: rand(0.5, 1));
        ///        _writing = false;
        ///                        
        ///        console.write(&quot;f [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ReadersWriterResource {
            get {
                return ResourceManager.GetString("ReadersWriterResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //------------------------------------------------------------------------------
        ///// &lt;auto-generated&gt;
        /////     This code was generated by a tool.
        /////     Runtime Version:4.0.30319.42000
        /////
        /////     Changes to this file may cause incorrect behavior and will be lost if
        /////     the code is regenerated.
        ///// &lt;/auto-generated&gt;
        /////------------------------------------------------------------------------------
        ///
        ///namespace Excess.Web.Resources {
        ///    using System;
        ///    
        ///    
        ///    /// &lt;summary&gt;
        ///    ///   A strongly- [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Samples_Designer {
            get {
                return ResourceManager.GetString("Samples_Designer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to var santa = spawn&lt;santa&gt;();
        ///var reindeer = spawn&lt;reindeer&gt;( 8).ToArray();
        ///var elves = spawn&lt;elf&gt;( 10).ToArray();
        ///
        ///var rnames = [&quot;Dasher&quot;, &quot;Dancer&quot;, &quot;Prancer&quot;, &quot;Vixen&quot;, &quot;Comet&quot;, &quot;Cupid&quot;, &quot;Dunder&quot;, &quot;Rudolph&quot;];
        ///var enames = [&quot;Alabaster&quot;, &quot;Bushy&quot;, &quot;Pepper&quot;, &quot;Shinny&quot;, &quot;Sugarplum&quot;, &quot;Wunorse&quot;, &quot;Buddy&quot;, &quot;Kringle&quot;, &quot;Tinsel&quot;, &quot;Jangle&quot;];
        ///
        ///start(santa);
        ///
        ///for(var i = 0; i &lt; rnames.Length; i++)
        ///    start(reindeer[i], santa, rnames[i]);
        ///
        ///for (var i = 0; i &lt; enames.Length; i++)
        ///    start(elves[i], santa, ename [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SantaCreaturesApp {
            get {
                return ResourceManager.GetString("SantaCreaturesApp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to concurrent class reindeer
        ///{
        ///    santa _santa;
        ///    string _name;                    
        ///
        ///    [Forever]    
        ///    void main(santa s, string name)
        ///    {
        ///        _santa = s;
        ///        _name = name;
        /// 
        ///        {vacation()}
        ///    }
        ///
        ///    public void unharness()
        ///    {
        ///        console.write(_name + &quot;: job well done&quot;);
        ///    }
        ///
        ///    private void vacation()
        ///    {
        ///        await wait(sec: rand(3, 7))
        ///                -&gt; console.write(_name + &quot;: back from vacation&quot;)
        ///                -&gt; (_santa.reindeer_back(this) [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SantaCreaturesCreatures {
            get {
                return ResourceManager.GetString("SantaCreaturesCreatures", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to concurrent class santa
        ///{
        ///    List&lt;reindeer&gt; _reindeer = new List&lt;reindeer&gt;();
        ///    List&lt;elf&gt;      _elves = new List&lt;elf&gt;();
        ///                    
        ///    private bool isDelivering { get {return _reindeer.Count == 8;} }
        ///                    
        ///    public void reindeer_back(reindeer r)
        ///    {
        ///        _reindeer.Add(r);
        ///        if (isDelivering)
        ///        {
        ///            await cancel_elves();
        ///
        ///            console.write(&quot;Santa: Off to deliver toys!&quot;);
        ///            await wait(sec:rand(5, 10));
        ///            consol [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SantaCreaturesSanta {
            get {
                return ResourceManager.GetString("SantaCreaturesSanta", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to var santa = spawn&lt;santa&gt;();
        ///var shop = spawn&lt;shop&gt;();
        ///
        ///start(santa);
        ///start(shop, santa);.
        /// </summary>
        internal static string SantaShopApp {
            get {
                return ResourceManager.GetString("SantaShopApp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to concurrent class santa
        ///{
        ///    bool _delivering = false;
        ///    bool _helping = false;
        ///                    
        ///    public void reindeer()
        ///    {
        ///        _delivering = true;
        ///        if (!_helping)
        ///            await deliver();    
        ///    }
        ///
        ///    public void elves()
        ///    {
        ///        if (_delivering)
        ///        {
        ///            console.write(&quot;elves: nobody home, back to work!&quot;);
        ///            return;
        ///        }
        ///
        ///        console.write(&quot;santa: hey guys, need help?&quot;);
        ///                        
        ///        _helping = true; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SantaShopSanta {
            get {
                return ResourceManager.GetString("SantaShopSanta", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to concurrent class shop
        ///{
        ///    santa _santa;
        ///    int   _reindeer = 0;
        ///    int   _elves    = 0;
        ///    void main(santa s)
        ///    {
        ///        _santa = s;
        ///                        
        ///        {reindeer() &amp; elves()}
        ///    }
        ///
        ///    [Forever]
        ///    public void reindeer()
        ///    {
        ///        console.write(&quot;reindeer: back from vacation&quot;);
        ///        _reindeer++;
        ///
        ///        if (_reindeer == 8)
        ///        {
        ///            await _santa.reindeer();
        ///            _reindeer = 0;
        ///        }
        ///
        ///        {wait(sec: rand(2, 3))}
        ///    }
        ///
        ///    [ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SantaShopShop {
            get {
                return ResourceManager.GetString("SantaShopShop", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to concurrent class philosopher 
        ///{
        ///    [Forever]
        ///    void main(string name, chopstick left, chopstick right) 
        ///    {
        ///        _name  = name;
        ///        _left  = left;
        ///        _right = right;
        ///                               
        ///        {hungry()}
        ///    }
        ///	
        ///    void hungry()
        ///    {
        ///        console.write(_name + &quot; is hungry&quot;);
        ///
        ///        try
        ///        {
        ///            await ((_left.acquire(this) &amp; _right.acquire(this))
        ///                  | timeout(sec: 2.1))
        ///                      -&gt; eat();
        ///        }
        ///        cat [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string StarvingPhilosophers {
            get {
                return ResourceManager.GetString("StarvingPhilosophers", resourceCulture);
            }
        }
    }
}
