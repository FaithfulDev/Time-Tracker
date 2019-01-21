# Changelog
This document tracks changes parallel to development and contains unreleased changes.

## v0.1.0.0 (17.01.2019)
### New Features
* Simple interface that is opened via tray menu icon; and disappears as soon as you done with it 
* Track time by clicking on the start/stop button
* Data is saved to a local SQLite database

## v0.2.0.0 (Unreleased)
### New Features
* Calculate total worked time (current day) and over time (for all days).
* Dark App Theme (there will be others later on).
* New Setting: Make window movable
    * User can decide if the window is fixed at its location at the corner of the desktop or freely movable.
### Bug Fixes
* End time is now always set to `now-1` to align with the start time of the next timer.
* Start and End time are now saved in 24h format.