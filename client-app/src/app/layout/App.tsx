import React, { Fragment, useEffect, useState } from 'react';
import 'semantic-ui-css/semantic.min.css';
import { Container } from 'semantic-ui-react';
import { Activity } from '../models/activity';
import NavBar from './NavBar';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import agent from '../api/agents';
import LoadingComponent from './LoadingComponent';
import { useStore } from '../strores/store';
import { observer } from 'mobx-react-lite';
import { Routes, Route, useLocation } from "react-router-dom";
import HomePage from '../../features/home/HomePage';
import ActivityForm from '../../features/activities/form/ActivityForm';
import ActivityDetails from '../../features/activities/details/ActivityDetails';
import TestErrors from '../../features/errors/Error';
import { ToastContainer } from 'react-toastify';
import NotFound from '../../features/errors/NotFound';
import ServerError from '../../features/errors/ServerError';
function App() {
  const location = useLocation();

  return (
    <Fragment>
      <ToastContainer position='bottom-right' hideProgressBar/>
      <NavBar />
      <Container style={{ marginTop: "7em" }}>
        <Routes>
          <Route path='/' Component={HomePage} />
          <Route path='/activities' Component={ActivityDashboard} />
          <Route path='/activities/:id' Component={ActivityDetails} />
          <Route key={location.key} path='/createActivity' Component={ActivityForm} />
          <Route key={location.key} path='/manage/:id' Component={ActivityForm} />
          <Route path='/errors' Component={TestErrors} />
          <Route path='/server-error' Component={ServerError} />
          <Route path='*' Component={NotFound}/>
        </Routes>
        
      </Container>
      
    </Fragment>
  );
}

export default observer(App);
