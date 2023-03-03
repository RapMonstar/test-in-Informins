# test-in-Informins
whats the problem we need to solve?
why CI/CD?  why build pipeline?

it is an automated build, verification and publish of a complex system
no need for people to access production server - security
takes care of synch - main issue (somebody added a method to an API and not everybody re-compiled re-tested etc)

enables to do tens of publishes a day easily!

enables to test branch/per new feature project common test deployment
say Jack Joe and Jim develop new feature so they create in each subsystem branch NEW_FEATURE
at some point they want to see the entire system.
They each make sure they pushed to bitbucket and then siply remotely call teamcity with a branch name as a parameter!!!

if needed handles (the ci/cd tool) multiple servers for build

we are really a very good non-contrived example of a very complex configuration

suppose we are ozon.com and sell something.
One system is the UI were the catalog and the shopping cart are
But when the purchase is being sone other systems need to be accessed - billing (BS) and fulfillment to name a few
for example if we purchased something to be paid in several installments
the schedule needs to be calculated and created in BS and also printed in PDF for the customer
Now we also understand we dont want to go directly to the billing db and we also dont want to replicate the logic
of say making payment schedule etc.
So billing system has a special API library it "exports" to all other systems the use to create a schedule, create
a payment info, so credit card transaction

same for the other systems.

sharing the logic that is implemented once in one place is the least error prone and most productive way of doing things
but it does involve some synchronization
suppose we changed some logic in the BS and pushed to master branch ready to publish to test
we want to re-export that export API library to other dependent systems!
we checkout each system
we copy that export lib into common libs area (for each system)
we also need to (possibly) recompile that dependent system and then we publish to test
we also need to run unit tests
then we check into master branch

if all systems are fine we are ready to push master to production branch
and finally publish to staging

Thats the broad picture
that will already be something!!!

what next? within each system we need to configure fluent database migration step
we also need to address roll-back for a single system

question? does build pipeline have a transaction scope to be able to rollback
everything published by a certain moment (on error)?

Tools involved

we need to access bitbucket pulling a branch for a certain repository - remote GIT calls
examples:
PS D:\src> cd testpublish
PS D:\src\testpublish> git add .
PS D:\src\testpublish> git commit -m "message"
PS D:\src\testpublish> git push https://informins-admin@bitbucket.org/informins/testpublish.git master


we need to copy files in powershell - powershell

we need to start cake/msbuild for a repository/vs solution

we need to check results of the build

we need to publish using msdeploy

we need to call unit tests and scan results for errors


all of that stuff is piece-wise on google

++++++++++++++++++++++++++++++++++++++
now how to do it and keep it trackable?

first we give you a bitbucket.org account - cloud version control system GIT based

create in VS 3 solutions A B C each:  console app plus library that retrieves something from db
B and C will have folder called common_libs where will be library A for B and A and B for C
so B and C dependent on A and C on B

each write unit test project

so change library in A
first w/o fluent migration

see how it propagates to C and publishes
run all three from script and see the changes
