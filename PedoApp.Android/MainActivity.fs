﻿// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace PedoApp.Android

open System

open Android.App
open Android.Content
open Android.Content.PM
open Android.Runtime
open Android.Views
open Android.Widget
open Android.OS
open Xamarin.Forms.Platform.Android


type PedometerListener(onChanged) =
    inherit Java.Lang.Object()
    interface Android.Hardware.ISensorEventListener with
        member _.OnAccuracyChanged(_, _) = ()
        member _.OnSensorChanged e =
            // steps since device boot
            onChanged <| int e.Values.[0]
type PedometerAndroid() =
    let manager =  
        Context.SensorService
        |> Xamarin.Essentials.Platform.CurrentActivity.GetSystemService
        :?> Android.Hardware.SensorManager
    let sensor = manager.GetDefaultSensor Android.Hardware.SensorType.StepCounter
    let event = Event<int>()         
    do
        if sensor <> null then
            manager.RegisterListener(new PedometerListener(event.Trigger), sensor, Android.Hardware.SensorDelay.Fastest)
            |> ignore
    interface PedoApp.App.Pedometer with
        member _.IsSupported = sensor <> null
        member _.Step = event.Publish
[<UsesPermission("android.permission.ACTIVITY_RECOGNITION")>]
[<UsesFeature(Name=Android.Hardware.Sensor.StringTypeStepCounter, Required=true)>]
do ()

[<Activity (Label = "PedoApp.Android", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = (ConfigChanges.ScreenSize ||| ConfigChanges.Orientation))>]
type MainActivity() =
    inherit FormsAppCompatActivity()
    override this.OnCreate (bundle: Bundle) =
        FormsAppCompatActivity.TabLayoutResource <- Resources.Layout.Tabbar
        FormsAppCompatActivity.ToolbarResource <- Resources.Layout.Toolbar
        base.OnCreate (bundle)

        Xamarin.Essentials.Platform.Init(this, bundle)

        Xamarin.Forms.Forms.Init (this, bundle)
        Xamarin.Forms.DependencyService.Register<PedometerAndroid>()
        OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init()
        let appcore  = new PedoApp.App()
        this.LoadApplication (appcore)

    override this.OnRequestPermissionsResult(requestCode: int, permissions: string[], [<GeneratedEnum>] grantResults: Android.Content.PM.Permission[]) =
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults)

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults)
