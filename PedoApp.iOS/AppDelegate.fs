// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace PedoApp.iOS

open System
open UIKit
open Foundation
open Xamarin.Forms
open Xamarin.Forms.Platform.iOS

type PedometeriOS() =
    let pedometer = new CoreMotion.CMPedometer()
    let event = Event<int>()
    // steps from midnight
    do pedometer.StartPedometerUpdates(DateTime.Today.ToNSDate(),
        Action<_, _>(fun data _ -> event.Trigger data.NumberOfSteps.Int32Value))
    interface PedoApp.App.Pedometer with member _.Step = event.Publish

[<Register ("AppDelegate")>]
type AppDelegate () =
    inherit FormsApplicationDelegate ()

    override this.FinishedLaunching (app, options) =
        Forms.Init()
        OxyPlot.Xamarin.Forms.Platform.iOS.PlotViewRenderer.Init()
        let appcore = new PedoApp.App()
        this.LoadApplication (appcore)
        base.FinishedLaunching(app, options)

module Main =
    [<EntryPoint>]
    let main args =
        UIApplication.Main(args, null, "AppDelegate")
        0

